using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Delta.Dnevnik.Models;

public class Schedule
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("activities")]
    public Activity[] Activities { get; set; }

    [JsonPropertyName("has_homework")]
    public bool HasHomework { get; set; }
}

public class Activity
{
    [JsonPropertyName("type")]
    public TypeEnum Type { get; set; }

    [JsonPropertyName("info")]
    public string Info { get; set; }

    [JsonPropertyName("begin_utc")]
    public long BeginUtc { get; set; }

    [JsonPropertyName("end_utc")]
    public long EndUtc { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("begin_time")]
    public string BeginTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("end_time")]
    public string EndTime { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("room_number")]
    public string RoomNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("room_name")]
    public string RoomName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("building_name")]
    public string BuildingName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("lesson")]
    public Lesson Lesson { get; set; }

    [JsonPropertyName("homework_presence_status_id")]
    public long? HomeworkPresenceStatusId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
}

public class Lesson
{
    [JsonPropertyName("schedule_item_id")]
    public long ScheduleItemId { get; set; }

    [JsonPropertyName("subject_id")]
    public long SubjectId { get; set; }

    [JsonPropertyName("subject_name")]
    public string SubjectName { get; set; }

    [JsonPropertyName("course_lesson_type")]
    public object CourseLessonType { get; set; }

    [JsonPropertyName("teacher")]
    public Teacher Teacher { get; set; }

    [JsonPropertyName("marks")]
    public Mark[] Marks { get; set; }

    [JsonPropertyName("homework")]
    public string Homework { get; set; }

    [JsonPropertyName("link_types")]
    public string[] LinkTypes { get; set; }

    [JsonPropertyName("materials_count")]
    public MaterialsCount MaterialsCount { get; set; }

    [JsonPropertyName("lesson_type")]
    public string LessonType { get; set; }

    [JsonPropertyName("lesson_education_type")]
    public string LessonEducationType { get; set; }

    [JsonPropertyName("evaluation")]
    public object Evaluation { get; set; }

    [JsonPropertyName("absence_reason_id")]
    public long? AbsenceReasonId { get; set; }

    [JsonPropertyName("bell_id")]
    public long? BellId { get; set; }

    [JsonPropertyName("replaced")]
    public bool Replaced { get; set; }

    [JsonPropertyName("homework_count")]
    public HomeworkCount HomeworkCount { get; set; }

    [JsonPropertyName("esz_field_id")]
    public object EszFieldId { get; set; }

    [JsonPropertyName("is_cancelled")]
    public bool? IsCancelled { get; set; }

    [JsonPropertyName("is_missed_lesson")]
    public bool IsMissedLesson { get; set; }

    [JsonPropertyName("is_virtual")]
    public bool IsVirtual { get; set; }
}

public class HomeworkCount
{
    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    [JsonPropertyName("ready_count")]
    public long ReadyCount { get; set; }
}

public class Mark
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Value { get; set; }

    [JsonPropertyName("values")]
    public Value[] Values { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [JsonPropertyName("weight")]
    public long Weight { get; set; }

    [JsonPropertyName("point_date")]
    public DateTimeOffset PointDate { get; set; }

    [JsonPropertyName("control_form_name")]
    public string ControlFormName { get; set; }

    [JsonPropertyName("comment_exists")]
    public bool CommentExists { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("criteria")]
    public object[] Criteria { get; set; }

    [JsonPropertyName("is_exam")]
    public bool IsExam { get; set; }

    [JsonPropertyName("is_point")]
    public bool IsPoint { get; set; }

    [JsonPropertyName("original_grade_system_type")]
    public string OriginalGradeSystemType { get; set; }
}

public class Value
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("nmax")]
    public long Nmax { get; set; }

    [JsonPropertyName("grade")]
    public Grade Grade { get; set; }

    [JsonPropertyName("grade_system_id")]
    public long GradeSystemId { get; set; }

    [JsonPropertyName("grade_system_type")]
    public string GradeSystemType { get; set; }
}

public class Grade
{
    [JsonPropertyName("origin")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Origin { get; set; }

    [JsonPropertyName("five")]
    public long Five { get; set; }

    [JsonPropertyName("hundred")]
    public long Hundred { get; set; }
}

public class MaterialsCount
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("attachments")]
    public long? Attachments { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("execute")]
    public long? Execute { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("test_spec_binding")]
    public long? TestSpecBinding { get; set; }
}

public class Teacher
{
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("middle_name")]
    public string MiddleName { get; set; }

    [JsonPropertyName("birth_date")]
    public object BirthDate { get; set; }

    [JsonPropertyName("sex")]
    public object Sex { get; set; }

    [JsonPropertyName("user_id")]
    public object UserId { get; set; }
}

public enum TypeEnum { Break, Lesson };

internal static class Converter
{
    public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
    {
        Converters =
        {
            TypeEnumConverter.Singleton,
            new DateOnlyConverter(),
            new TimeOnlyConverter(),
            IsoDateTimeOffsetConverter.Singleton
        },
    };
}

internal class ParseStringConverter : JsonConverter<long>
{
    public override bool CanConvert(Type t) => t == typeof(long);

    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        if (long.TryParse(value, out var l))
        {
            return l;
        }
        throw new Exception("Cannot unmarshal type long");
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToString(), options);
        return;
    }

    public static readonly ParseStringConverter Singleton = new();
}

internal class TypeEnumConverter : JsonConverter<TypeEnum>
{
    public override bool CanConvert(Type t) => t == typeof(TypeEnum);

    public override TypeEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        return value switch
        {
            "BREAK" => TypeEnum.Break,
            "LESSON" => TypeEnum.Lesson,
            _ => throw new Exception("Cannot unmarshal type TypeEnum")
        };
    }

    public override void Write(Utf8JsonWriter writer, TypeEnum value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case TypeEnum.Break:
                JsonSerializer.Serialize(writer, "BREAK", options);
                return;
            case TypeEnum.Lesson:
                JsonSerializer.Serialize(writer, "LESSON", options);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, "Cannot marshal type TypeEnum");
        }
    }

    public static readonly TypeEnumConverter Singleton = new();
}
    
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string _serializationFormat;
    public DateOnlyConverter() : this(null) { }

    public DateOnlyConverter(string? serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "yyyy-MM-dd";
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat;

    public TimeOnlyConverter() : this(null) { }

    public TimeOnlyConverter(string? serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}

internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

    private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

    private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
    private string? _dateTimeFormat;
    private CultureInfo? _culture;

    public DateTimeStyles DateTimeStyles
    {
        get => _dateTimeStyles;
        set => _dateTimeStyles = value;
    }

    public string? DateTimeFormat
    {
        get => _dateTimeFormat ?? string.Empty;
        set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
    }

    public CultureInfo Culture
    {
        get => _culture ?? CultureInfo.CurrentCulture;
        set => _culture = value;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
            || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
        {
            value = value.ToUniversalTime();
        }

        var text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

        writer.WriteStringValue(text);
    }

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateText = reader.GetString();

        if (string.IsNullOrEmpty(dateText)) 
            return default;
        
        if (!string.IsNullOrEmpty(_dateTimeFormat))
        {
            return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
        }

        return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);

    }


    public static readonly IsoDateTimeOffsetConverter Singleton = new();
}