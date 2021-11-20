using System;
using System.Collections.Generic;
using System.Text;

namespace SceneSource
{
    public interface ISourceItem
    {
        bool CanHaveChildren { get; }
        bool HasChildren { get; }

        IEnumerable<SourceItem> Flatten();
        List<SourceItem> FlattenL();
    }
}
