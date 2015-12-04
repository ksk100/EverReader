using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Models
{
    public class TagOperationDefinition
    {
        public int Id { get; set; }
        public TagOperationType Type { get; set; }
        public string TagGuid { get; set; }
    }
}
