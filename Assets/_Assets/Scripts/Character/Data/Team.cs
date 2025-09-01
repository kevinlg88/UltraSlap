using UnityEngine;

public class Team
{
    private TeamEnum teamEnum;
    private Color color;
    private int score;
    private GameObject gameObjectUI;

    public TeamEnum TeamEnum { get => teamEnum; set => teamEnum = value; }
    public Color Color { get => color; set => color = value; }
    public int Score { get => score; set => score = value; }
    public GameObject GameObjectUI { get => gameObjectUI; set => gameObjectUI = value; }

    public Team(TeamEnum teamEnum, Color color, GameObject gameObjectUI = null, int score = 0)
    {
        this.teamEnum = teamEnum;
        this.color = color;
        this.gameObjectUI = gameObjectUI;
        this.score = score;
    }

}
