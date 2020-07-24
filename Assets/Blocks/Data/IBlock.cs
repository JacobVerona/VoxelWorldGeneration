namespace Unit.Blocks.Data
{
    interface IBlock
    {

         bool IsSolid { get; }
         bool IsTransparent { get; }
    }
}
