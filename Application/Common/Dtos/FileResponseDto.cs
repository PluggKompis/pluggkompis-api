namespace Application.Common.Dtos
{
    public class FileResponseDto
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = "file.bin";
    }
}
