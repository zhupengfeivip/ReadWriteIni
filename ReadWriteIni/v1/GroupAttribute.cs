using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWriteIni.v1
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute(string group, string name, string comment)
        {
            Group = group;
            Name = name;
            Comment = comment;
        }


        public GroupAttribute(string group, string name) : this(group, name, null)
        { 
        
        }

        public GroupAttribute(string group) : this(group, null, null)
        {

        }

        public GroupAttribute() : this(null, null, null)
        {

        }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue { get; set; }

    }
}
