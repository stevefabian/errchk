namespace errchk.Models 
{
    public class Row {
        public int Line { get; set; }
        public string Raw { get; set; }
        public string ec_username { get; set; } 
        public string sdp_id { get; set; }
        public string efd { get; set; }
        public string type { get; set; }
        public string details { get; set; }
    }

    public class FileInfo {
        public string Filename { get; set; }
        public string Type { get; set; }
        public string FileType { get; set; }
        public string ErrorDate { get; set; }
    }

}