using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LyumixLauncher
{
    public class PropertyItem
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public JsonElement Value { get; set; } // This is of type object because Value could be different types (string, bool, etc.)
        public string Type { get; set; }
        public string Options { get; set; } // Options is optional and exists only for ComboBox items
        public bool InSecure { get; set; } // InSecure is optional and exists only for TextBox items
    }
}
