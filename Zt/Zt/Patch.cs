using System.Collections.Generic;

namespace JeremyAnsel.Xwa.ExePatcher
{
    public sealed class Patch
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public IList<PatchItem> Items { get; } = new List<PatchItem>();
    }
}
