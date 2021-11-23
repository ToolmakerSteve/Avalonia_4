using System;
using System.Collections.Generic;
using System.Text;

namespace SceneSource
{
    /// <summary>
    /// "Source" data. E.g. if wall is drawn as a 2D line across a terrain,
    /// this (or a subclass) efficiently represents that line.
    /// It will then get converted into a corresponding 3D model in the 3D scene.
    /// "SourceItem" represents data that does not contain Children.
    /// </summary>
    public class SourceItem : ISourceItem
    {
        #region --- data, new ----------------------------------------
        public virtual bool CanHaveChildren => false;

        public virtual bool HasChildren => false;

        //MAYBE_IN_SUBCLASS public readonly Dictionary<string, object> Data = new Dictionary<string, object>();


        public SourceItem()
        {
        }
        #endregion


        #region --- public methods ----------------------------------------
        /// <summary>
        /// Returns self.
        /// </summary>
        public virtual IEnumerable<SourceItem> Flatten()
        {
            yield return this;
        }

        public List<SourceItem> FlattenL()
        {
            var all = new List<SourceItem>();
            Flatten(ref all);
            return all;
        }


        public virtual void Flatten(ref List<SourceItem> all)
        {
            all.Add(this);
        }
        #endregion
    }
}
