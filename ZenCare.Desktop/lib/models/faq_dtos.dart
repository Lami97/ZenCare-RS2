import 'admin_models.dart';

class FaqDto implements AdminEntity {
  const FaqDto({
    required this.id,
    required this.question,
    required this.answer,
    required this.faqCategoryId,
    required this.faqCategoryName,
    required this.displayOrder,
    required this.isActive,
    required this.createdAt,
    this.updatedAt,
  });
  @override
  final int id;
  final String question;
  final String answer;
  final int faqCategoryId;
  final String faqCategoryName;
  final int displayOrder;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;
  factory FaqDto.fromJson(JsonMap json) => FaqDto(
    id: jsonInt(json['id']),
    question: json['question']?.toString() ?? '',
    answer: json['answer']?.toString() ?? '',
    faqCategoryId: jsonInt(json['faqCategoryId']),
    faqCategoryName: json['faqCategoryName']?.toString() ?? '',
    displayOrder: jsonInt(json['displayOrder']),
    isActive: jsonBool(json['isActive']),
    createdAt:
        jsonDateTime(json['createdAt']) ??
        DateTime.fromMillisecondsSinceEpoch(0, isUtc: true),
    updatedAt: jsonDateTime(json['updatedAt']),
  );
  @override
  Object? formValue(String key) => switch (key) {
    'question' => question,
    'answer' => answer,
    'faqCategoryId' => faqCategoryId,
    'displayOrder' => displayOrder,
    'isActive' => isActive,
    _ => null,
  };
}

class FaqInsertDto implements AdminWriteDto {
  const FaqInsertDto({
    required this.question,
    required this.answer,
    required this.faqCategoryId,
    required this.displayOrder,
    required this.isActive,
  });
  final String question;
  final String answer;
  final int faqCategoryId;
  final int displayOrder;
  final bool isActive;
  @override
  JsonMap toJson() => {
    'question': question,
    'answer': answer,
    'faqCategoryId': faqCategoryId,
    'displayOrder': displayOrder,
    'isActive': isActive,
  };
}

class FaqUpdateDto extends FaqInsertDto {
  const FaqUpdateDto({
    required this.id,
    required super.question,
    required super.answer,
    required super.faqCategoryId,
    required super.displayOrder,
    required super.isActive,
  });
  final int id;
  @override
  JsonMap toJson() => {...super.toJson(), 'id': id};
}
