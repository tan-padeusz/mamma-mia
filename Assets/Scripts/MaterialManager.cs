using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; }

    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;
    
    private void Start()
    {
        MaterialManager.Instance = this;
    }

    public Material GetMaterialByTeam(Team team)
    {
        return team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };
    }
}
