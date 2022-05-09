namespace TnyFramework.Codec
{

    public class MimeTypes : MimeType<MimeTypes>
    {
        public const string JSON_TYPE = "json";

        public const string PROTOBUF_TYPE = "protobuf";

        public const string TYPE_PROTOBUF_TYPE = "type-protobuf";

        public static readonly MimeType JSON = Of(100, JSON_TYPE);

        public static readonly MimeType PROTOBUF = Of(200, PROTOBUF_TYPE);

        public static readonly MimeType TYPE_PROTOBUF = Of(300, TYPE_PROTOBUF_TYPE);
    }

}
