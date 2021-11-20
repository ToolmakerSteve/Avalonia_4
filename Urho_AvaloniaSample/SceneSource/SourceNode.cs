using System.Collections.Generic;

namespace SceneSource
{
    /// <summary>
    /// "Source" data.
    /// "SourceNode" represents data that can contain Children.
    /// </summary>
    public class SourceNode : SourceItem
    {
        #region "-- data, new --"
        public override bool CanHaveChildren => true;

        public override bool HasChildren => Children.Count > 0;

        public List<SourceItem> Children = new List<SourceItem>();


        public SourceNode()
        {
        }
        #endregion


        #region "-- public methods --"
        public void Add(SourceItem child)
        {
            Children.Add(child);
        }

        public void Remove(SourceItem child)
        {
            Children.Remove(child);
        }


        /// <summary>
        /// Returns self, then returns all children (and their descendents).
        /// </summary>
        public override IEnumerable<SourceItem> Flatten()
        {
            yield return this;

            if (HasChildren)
            {
                Queue<SourceItem> more = new Queue<SourceItem>(Children);
                while (more.Count > 0)
                {
                    SourceItem item = more.Dequeue();
                    yield return item;

                    if (item is SourceNode node && node.HasChildren)
                        foreach (var child in node.Children)
                            more.Enqueue(child);
                }
            }
        }

        public override void Flatten(ref List<SourceItem> all)
        {
            all.Add(this);

            foreach (var child in Children)
                child.Flatten(ref all);
        }


        //public void Dispose()
        //{
        //    Children.Clear();
        //}
        #endregion
    }
}
