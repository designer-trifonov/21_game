using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Экран выбора девушки.
/// Карточки создаются через CardFactory — никаких скриптов на префабах.
/// </summary>
public class GirlSelectScreen : MonoBehaviour
{
    [Header("UI")]
    public Transform gridContent;

    void OnEnable()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        foreach (Transform child in gridContent)
            Destroy(child.gameObject);

        var girls = DataRepository.LoadAllGirls();

        foreach (var girl in girls)
        {
            var captured = girl;
            CardFactory.Create(
                parent:    gridContent,
                imagePath: captured.profilePhoto,
                label:     captured.name,
                size:      new Vector2(300, 400),
                onClick:   () => OnGirlSelected(captured)
            );
        }
    }

    void OnGirlSelected(GirlData girl)
    {
        GameState.StartGame(girl);
        UIManager.Instance.ShowDifficulty();
    }
}
