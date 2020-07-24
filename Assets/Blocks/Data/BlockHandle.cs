
namespace Unit.Blocks.Data
{
    public abstract class BlockHandle : IBlock
    {
        public abstract string BlockID { get; set; }
        public abstract bool IsSolid { get; }
        public abstract bool IsTransparent { get; }
        public abstract bool HasModel { get; }
    }
}

