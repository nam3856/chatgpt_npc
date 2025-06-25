public enum EPosition
{
    Pitcher,
    Catcher,
    Infielder,
    Outfielder,
    ManagerCandidate
}

public class HeroineGameStats
{
    public int Batting;
    public int Pitching;
    public int Defense;
    public int Speed;
    public int Stamina;
    public int GameSense;

    public int Leadership;
    public int Strategy;
    public int Motivation;
    public int Focus;
    public int CatchSense;

    public HeroineGameStats(
        int batting, int pitching, int defense, int speed, int stamina, int gameSense,
        int leadership, int strategy, int motivation, int focus, int catchSense)
    {
        Batting = batting;
        Pitching = pitching;
        Defense = defense;
        Speed = speed;
        Stamina = stamina;
        GameSense = gameSense;

        Leadership = leadership;
        Strategy = strategy;
        Motivation = motivation;
        Focus = focus;
        CatchSense = catchSense;
    }
}
