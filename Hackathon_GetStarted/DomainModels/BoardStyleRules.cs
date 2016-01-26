using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon_GetStarted
{
    class BoardStyleFillClauses
    {
        public string fieldName { get; set; }
        public int index { get; set; }
        public string logicalOperator { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
        public BoardStyleFillClauses(string _fieldName, int _index, string _logicalOperator, string _operator_, string _value)
        {
            this.fieldName = _fieldName;
            this.index = _index;
            this.logicalOperator = _logicalOperator;
            this.@operator = _operator_;
            this.value = _value;
        }
    }
    class BoardStyleFillSettings
    {
        [JsonProperty(PropertyName = "background-color")]
        public string backgroundcolor { get; set; }
        [JsonProperty(PropertyName = "title-color")]
        public string titlecolor { get; set; }
        public BoardStyleFillSettings(string _backgroundcolor, string _titlecolor)
        {
            this.backgroundcolor = _backgroundcolor;
            this.titlecolor = _titlecolor;
        }
    }
    class BoardStyleFill
    {
        public string name { get; set; }
        public string isEnabled { get; set; }
        public string filter { get; set; }
        public List<BoardStyleFillClauses> clauses { get; set; }
        public BoardStyleFillSettings settings { get; set; }
        public BoardStyleFill(string _name, string _isEnabled, string _filter, List<BoardStyleFillClauses> _Clauses, BoardStyleFillSettings _settings)
        {
            this.name = _name;
            this.isEnabled = _isEnabled;
            this.filter = _filter;
            this.clauses = _Clauses;
            this.settings = _settings;
        }
    }
    class BoardStyleTagStyleSettings
    {
        [JsonProperty(PropertyName = "background-color")]
        public string backgroundcolor { get; set; }
        public string color { get; set; }
        public BoardStyleTagStyleSettings(string _backgroundcolor, string _color)
        {
            this.backgroundcolor = _backgroundcolor;
            this.color = _color;
        }
    }
    class BoardStyleTagStyle
    {
        public string name { get; set; }
        public string isEnabled { get; set; }
        public BoardStyleTagStyleSettings settings { get; set; }
        public BoardStyleTagStyle(string _name, string _isEnabled, BoardStyleTagStyleSettings _settings)
        {
            this.name = _name;
            this.isEnabled = _isEnabled;
            this.settings = _settings;
        }
    }
    class BoardStyleRules
    {
        public List<BoardStyleFill> fill { get; set; }
        public List<BoardStyleTagStyle> tagStyle { get; set; }
        public BoardStyleRules(List<BoardStyleFill> _fill, List<BoardStyleTagStyle> _tagStyle)
        {
            this.fill = _fill;
            this.tagStyle = _tagStyle;
        }
    }
}
