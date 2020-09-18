using System;

namespace ShellDuo.Models
{
    public class Item : BaseModel
    {
        string id = string.Empty;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
    }
}