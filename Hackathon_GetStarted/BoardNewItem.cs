using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon_GetStarted
{
    class BoardNewItemProperty
    {
        public string op { get; set; }
        public string path { get; set; }
        public string value { get; set; }
        public BoardNewItemProperty(string _op, string _path, string _value)
        {
            this.op = _op;
            this.path = _path;
            this.value = _value;
        }
        
    }
    class BoardNewItem
    {
        public BoardNewItemProperty title { get; set; }
        public BoardNewItemProperty areatPath { get; set; }
        public BoardNewItemProperty teamProject { get; set; }
        public BoardNewItemProperty iterationPath { get; set; }
        public BoardNewItemProperty workItemType { get; set; }
        public BoardNewItemProperty state { get; set; }
        public BoardNewItemProperty assignedTo { get; set; }
        public BoardNewItemProperty tags { get; set; }
        public BoardNewItem(string _title, string _areaPath, string _teamProject, string _iterationPath, string _workItemType, string _state, string _assignedTo, string _tags)
        {
            this.title = new BoardNewItemProperty("add", "/fields/System.Title", _title);
            this.areatPath = new BoardNewItemProperty("add", "/fields/System.AreaPath", _areaPath);
            this.teamProject = new BoardNewItemProperty("add", "/fields/System.TeamProject", _teamProject);
            this.iterationPath = new BoardNewItemProperty("add", "/fields/System.IterationPath", _iterationPath);
            this.workItemType = new BoardNewItemProperty("add", "/fields/System.WorkItemType", _workItemType);
            this.state = new BoardNewItemProperty("add", "/fields/System.State", _state);
            this.assignedTo = new BoardNewItemProperty("add", "/fields/System.AssignedTo", _assignedTo);
            this.tags = new BoardNewItemProperty("add", "/fields/System.Tags", _tags);
        }


    }
}
