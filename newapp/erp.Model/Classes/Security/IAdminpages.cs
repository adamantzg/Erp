namespace erp.Model
{
    public interface IAdminpages
    {
        int page_id { get; set; }
        int? parent_id { get; set; }
        string page_title { get; set; }
        int? page_type { get; set; }
        string notes { get; set; }
        string page_URL { get; set; }
        string parameter1 { get; set; }
        string parameter1_value { get; set; }
        string URL_value { get; set; }
        bool? hide_menu { get; set; }
        string path { get; set; }
    }
}