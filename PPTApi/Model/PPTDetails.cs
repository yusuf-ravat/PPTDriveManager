using System.ComponentModel.DataAnnotations;

namespace PPTApi.Model
{
    public class PPTDetails
    {
        [Key]
        public int Id { get; set; }
        public string? PptFilePath { get; set; }
        //public string? PptFileName { get; set; }
        public byte[]? PptFileData { get; set; }
        public string? DriveFileId { get; set; }
        public string? DriveFilepath { get; set; }
    }
}
