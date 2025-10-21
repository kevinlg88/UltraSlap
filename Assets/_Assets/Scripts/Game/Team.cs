using UnityEngine;

public class Team
{
    private TeamEnum teamEnum;
    private Color color;

    public TeamEnum TeamEnum { get => teamEnum; set => teamEnum = value; }
    public Color Color { get => color; set => color = value; }

    public Team(TeamEnum teamEnum, Color color)
    {
        this.teamEnum = teamEnum;
        this.color = color;
    }

}
