namespace WebUI.DTOs
{
    public class StudentRegisterHelper
    {

        public List<XLevel> GetLevels()
        {
            List<XLevel> levels = new()
            {
                new XLevel() {Level = 100} ,
                new XLevel() {Level = 200} ,
                new XLevel() {Level = 300} ,
                new XLevel() {Level = 400} ,
                new XLevel() {Level = 500} ,
                new XLevel() {Level = 600} ,
                new XLevel() {Level = 700}
            };
            return levels;
        }

        public List<XGender> GetGender()
        {
            List<XGender> genders = new(){
                new XGender() {Gender = "Female"} ,
                new XGender() {Gender = "Male"}
            };
            return genders;
        }
    }




    public class XLevel
    {
        public int Level { get; set; }
    }

    public class XGender
    {
        public string Gender { get; set; }
    }

}
