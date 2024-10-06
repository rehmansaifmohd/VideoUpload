namespace VideoUpload.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public long Size { get; set; }
        public string FilePath { get; set; } = "";
    }
}
