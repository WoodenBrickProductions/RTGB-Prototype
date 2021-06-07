public class Position
{
    public int x { get; set; }
    public int y { get; set; }
    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Position Up = new Position(0,1);
    public static Position Right = new Position(1,0);
    public static Position Down = new Position(0,-1);
    public static Position Left = new Position(-1,0);

    public static Position operator +(Position p1, Position p2)
    {
        return new Position(p1.x + p2.x, p1.y + p2.y);
    }

    public override bool Equals(object obj)
    {
        if (!obj.GetType().Equals(typeof(Position)))
        {
            return false;
        }

        Position pos = (Position) obj;
        return (pos.x == x && pos.y == y);

    }
}
