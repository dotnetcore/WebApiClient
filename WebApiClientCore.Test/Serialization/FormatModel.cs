namespace WebApiClientCore.Test.Serialization
{
    public class FormatModel
    {
        public int Age { get; set; }

        public string? Name { get; set; }

        public override int GetHashCode()
        {
            return Age.GetHashCode() ^ (Name == null ? 0 : Name.GetHashCode());
        }

        public override bool Equals(object? obj)
        {
            return obj is FormatModel o && o.Age == this.Age && o.Name == this.Name;
        }
    }
}
