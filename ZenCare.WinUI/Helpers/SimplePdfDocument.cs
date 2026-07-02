using System.Globalization;
using System.Text;

namespace ZenCare.WinUI.Helpers;

public class SimplePdfDocument
{
    private const double PageWidth = 595;
    private const double PageHeight = 842;
    private const double LeftMargin = 50;
    private const double TopMargin = 60;
    private const double BottomMargin = 60;
    private const double LineHeight = 18;

    private readonly List<List<PdfTextLine>> _pages = new() { new List<PdfTextLine>() };
    private double _currentY = PageHeight - TopMargin;

    public void AddTitle(string text)
    {
        AddText(text, 18);
        AddBlankLine();
    }

    public void AddSection(string text)
    {
        AddBlankLine();
        AddText(text, 13);
    }

    public void AddText(string text)
    {
        AddText(text, 10);
    }

    public void AddTable(IEnumerable<string[]> rows)
    {
        foreach (var row in rows)
        {
            AddText(string.Join("    ", row.Select(CleanText)), 10);
        }
    }

    public void AddBlankLine()
    {
        _currentY -= LineHeight;
        EnsurePageSpace();
    }

    public void Save(string filePath)
    {
        var objects = new List<string>();
        var pageObjectNumbers = new List<int>();

        objects.Add("<< /Type /Catalog /Pages 2 0 R >>");
        objects.Add(string.Empty);
        objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

        foreach (var page in _pages)
        {
            var contentObjectNumber = objects.Count + 2;
            var pageObjectNumber = objects.Count + 1;
            pageObjectNumbers.Add(pageObjectNumber);

            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth.ToString(CultureInfo.InvariantCulture)} {PageHeight.ToString(CultureInfo.InvariantCulture)}] /Resources << /Font << /F1 3 0 R >> >> /Contents {contentObjectNumber} 0 R >>");
            objects.Add(CreateContentStream(page));
        }

        objects[1] = $"<< /Type /Pages /Kids [{string.Join(" ", pageObjectNumbers.Select(x => $"{x} 0 R"))}] /Count {pageObjectNumbers.Count} >>";

        WritePdf(filePath, objects);
    }

    private void AddText(string text, int fontSize)
    {
        EnsurePageSpace();
        _pages[^1].Add(new PdfTextLine(CleanText(text), LeftMargin, _currentY, fontSize));
        _currentY -= LineHeight;
    }

    private void EnsurePageSpace()
    {
        if (_currentY >= BottomMargin)
        {
            return;
        }

        _pages.Add(new List<PdfTextLine>());
        _currentY = PageHeight - TopMargin;
    }

    private static string CreateContentStream(IEnumerable<PdfTextLine> lines)
    {
        var content = new StringBuilder();

        foreach (var line in lines)
        {
            content.Append("BT ");
            content.Append(CultureInfo.InvariantCulture, $"/F1 {line.FontSize} Tf ");
            content.Append(CultureInfo.InvariantCulture, $"{line.X} {line.Y} Td ");
            content.Append('(');
            content.Append(EscapePdfText(line.Text));
            content.AppendLine(") Tj ET");
        }

        var contentText = content.ToString();
        return $"<< /Length {Encoding.ASCII.GetByteCount(contentText)} >>\nstream\n{contentText}endstream";
    }

    private static void WritePdf(string filePath, List<string> objects)
    {
        var builder = new StringBuilder();
        var offsets = new List<int> { 0 };

        builder.AppendLine("%PDF-1.4");

        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(builder.ToString()));
            builder.AppendLine($"{i + 1} 0 obj");
            builder.AppendLine(objects[i]);
            builder.AppendLine("endobj");
        }

        var xrefOffset = Encoding.ASCII.GetByteCount(builder.ToString());
        builder.AppendLine("xref");
        builder.AppendLine($"0 {objects.Count + 1}");
        builder.AppendLine("0000000000 65535 f ");

        foreach (var offset in offsets.Skip(1))
        {
            builder.AppendLine($"{offset:0000000000} 00000 n ");
        }

        builder.AppendLine("trailer");
        builder.AppendLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        builder.AppendLine("startxref");
        builder.AppendLine(xrefOffset.ToString(CultureInfo.InvariantCulture));
        builder.AppendLine("%%EOF");

        File.WriteAllText(filePath, builder.ToString(), Encoding.ASCII);
    }

    private static string CleanText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "-";
        }

        return text
            .Replace("č", "c", StringComparison.OrdinalIgnoreCase)
            .Replace("ć", "c", StringComparison.OrdinalIgnoreCase)
            .Replace("ž", "z", StringComparison.OrdinalIgnoreCase)
            .Replace("š", "s", StringComparison.OrdinalIgnoreCase)
            .Replace("đ", "d", StringComparison.OrdinalIgnoreCase);
    }

    private static string EscapePdfText(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }

    private sealed record PdfTextLine(string Text, double X, double Y, int FontSize);
}
