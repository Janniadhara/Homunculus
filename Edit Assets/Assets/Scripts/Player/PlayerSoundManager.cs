using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Sound is played with the event in an animation

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerSoundDataObject playerSoundData;

    public Transform playerTransform;
    public Terrain t;
    public int posX;
    public int posZ;
    public float[] textureValues;

    public int groundTexture;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        playerTransform = gameObject.transform;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        t = Terrain.activeTerrain;
        Debug.Log("Terrain should be loaded");
    }
    
    void WalkingSound()
    {
        CheckTerrain();
        if (CheckTerrain())
        {
            GetTerrainTexture();
            if (textureValues[0] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGrass[Random.Range(0, playerSoundData.WalkingGrass.Length)], 1);
            }
            if (textureValues[1] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGravel[Random.Range(0, playerSoundData.WalkingGravel.Length)], 1);
            }
            if (textureValues[2] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingDirt[Random.Range(0, playerSoundData.WalkingDirt.Length)], 1);
            }
            if (textureValues[3] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingStone[Random.Range(0, playerSoundData.WalkingStone.Length)], 1);
            }
            if (textureValues[4] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingDirt[Random.Range(0, playerSoundData.WalkingDirt.Length)], 1);
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingLeaves[Random.Range(0, playerSoundData.WalkingLeaves.Length)], 0.3f);
            }
            if (textureValues[5] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingWet[Random.Range(0, playerSoundData.WalkingWet.Length)], 1);
            }
        }
        else
        {
            GetGroundMaterial();
            if (groundTexture == 0)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGrass[Random.Range(0, playerSoundData.WalkingGrass.Length)], 1);
            }
            if (groundTexture == 1)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingStone[Random.Range(0, playerSoundData.WalkingStone.Length)], 1);
            }
            if (groundTexture == 2)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingWood[Random.Range(0, playerSoundData.WalkingWood.Length)], 1);
            }
        }
    }
    void SneakingSound()
    {
        CheckTerrain();
        if (CheckTerrain())
        {
            GetTerrainTexture();
            if (textureValues[0] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGrass[Random.Range(0, playerSoundData.WalkingGrass.Length)], 0.3f);
            }
            if (textureValues[1] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGravel[Random.Range(0, playerSoundData.WalkingGravel.Length)], 0.3f);
            }
            if (textureValues[2] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingDirt[Random.Range(0, playerSoundData.WalkingDirt.Length)], 0.3f);
            }
            if (textureValues[3] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingStone[Random.Range(0, playerSoundData.WalkingStone.Length)], 0.3f);
            }
            if (textureValues[4] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingDirt[Random.Range(0, playerSoundData.WalkingDirt.Length)], 0.3f);
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingLeaves[Random.Range(0, playerSoundData.WalkingLeaves.Length)], 0.1f);
            }
            if (textureValues[5] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingWet[Random.Range(0, playerSoundData.WalkingWet.Length)], 0.3f);
            }
        }
        else
        {
            GetGroundMaterial();
            if (groundTexture == 0)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingGrass[Random.Range(0, playerSoundData.WalkingGrass.Length)], 0.3f);
            }
            if (groundTexture == 1)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingStone[Random.Range(0, playerSoundData.WalkingStone.Length)], 0.3f);
            }
            if (groundTexture == 2)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.WalkingWood[Random.Range(0, playerSoundData.WalkingWood.Length)], 0.3f);
            }
        }
    }
    void RunningSound()
    {
        CheckTerrain();
        if (CheckTerrain())
        {
            GetTerrainTexture();
            if (textureValues[0] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningGrass[Random.Range(0, playerSoundData.RunningGrass.Length   )], 1);
            }
            if (textureValues[1] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningGravel[Random.Range(0, playerSoundData.RunningGravel.Length )], 1);
            }
            if (textureValues[2] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningDirt[Random.Range(0, playerSoundData.RunningDirt.Length)], 1);
            }
            if (textureValues[3] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningStone[Random.Range(0, playerSoundData.RunningStone.Length)], 1);
            }
            if (textureValues[4] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningDirt[Random.Range(0, playerSoundData.RunningDirt.Length)], 1);
                SoundManager.Instance.PlayEffect(playerSoundData.RunningLeaves[Random.Range(0, playerSoundData.RunningLeaves.Length)], 0.3f);
            }
            if (textureValues[5] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningWet[Random.Range(0, playerSoundData.RunningWet.Length)], 1);
            }
        }
        else
        {
            GetGroundMaterial();
            if (groundTexture == 0)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningGrass[Random.Range(0, playerSoundData.RunningGrass.Length)], 1);
            }
            if (groundTexture == 1)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningStone[Random.Range(0, playerSoundData.RunningStone.Length)], 1);
            }
            if (groundTexture == 2)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.RunningWood[Random.Range(0, playerSoundData.RunningWood.Length)], 1);
            }
        }
    }
    void JumpingSound()
    {
        CheckTerrain();
        if (CheckTerrain())
        {
            GetTerrainTexture();
            if (textureValues[0] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingGrass[Random.Range(0, playerSoundData.JumpingGrass.Length)], 1);
            }
            if (textureValues[1] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingGravel[Random.Range(0, playerSoundData.JumpingGravel.Length)], 1);
            }
            if (textureValues[2] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingDirt[Random.Range(0, playerSoundData.JumpingDirt.Length)], 1);
            }
            if (textureValues[3] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingStone[Random.Range(0, playerSoundData.JumpingStone.Length)], 1);
            }
            if (textureValues[4] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingDirt[Random.Range(0, playerSoundData.JumpingDirt.Length)], 1);
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingLeaves[Random.Range(0, playerSoundData.JumpingLeaves.Length)], 0.3f);
            }
            if (textureValues[5] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingWet[Random.Range(0, playerSoundData.JumpingWet.Length)], 1);
            }
        }
        else
        {
            GetGroundMaterial();
            if (groundTexture == 0)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingGrass[Random.Range(0, playerSoundData.JumpingGrass.Length)], 1);
            }
            if (groundTexture == 1)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingStone[Random.Range(0, playerSoundData.JumpingStone.Length)], 1);
            }
            if (groundTexture == 2)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.JumpingWood[Random.Range(0, playerSoundData.JumpingWood.Length)], 1);
            }
        }
    }
    void LandingSound()
    {
        CheckTerrain();
        if (CheckTerrain())
        {
            GetTerrainTexture();
            if (textureValues[0] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingGrass[Random.Range(0, playerSoundData.LandingGrass.Length)], 1);
            }
            if (textureValues[1] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingGravel[Random.Range(0, playerSoundData.LandingGravel.Length)], 1);
            }
            if (textureValues[2] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingDirt[Random.Range(0, playerSoundData.LandingDirt.Length)], 1);
            }
            if (textureValues[3] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingStone[Random.Range(0, playerSoundData.LandingStone.Length)], 1);
            }
            if (textureValues[4] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingDirt[Random.Range(0, playerSoundData.LandingDirt.Length)], 1);
                SoundManager.Instance.PlayEffect(playerSoundData.LandingLeaves[Random.Range(0, playerSoundData.LandingLeaves.Length)], 0.3f);
            }
            if (textureValues[5] > 0.5f)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingWet[Random.Range(0, playerSoundData.LandingWet.Length)], 1);
            }
        }
        else
        {
            GetGroundMaterial();
            if (groundTexture == 0)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingGrass[Random.Range(0, playerSoundData.LandingGrass.Length)], 1);
            }
            if (groundTexture == 1)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingStone[Random.Range(0, playerSoundData.LandingStone.Length)], 1);
            }
            if (groundTexture == 2)
            {
                SoundManager.Instance.PlayEffect(playerSoundData.LandingWood[Random.Range(0, playerSoundData.LandingWood.Length)], 1);
            }
        }
    }
    void SwimmingSound()
    {
        SoundManager.Instance.PlayEffect(playerSoundData.Swimming[Random.Range(0, playerSoundData.Swimming.Length)], 0.8f);
    }
    #region TerrainTexture
    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }
    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;
        Vector3 mapPosition = new Vector3(terrainPosition.x / t.terrainData.size.x, 0, terrainPosition.z / t.terrainData.size.z);
        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;
        posX = (int)xCoord;
        posZ = (int)zCoord;
    }
    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        textureValues[0] = aMap[0, 0, 0]; //grass
        textureValues[1] = aMap[0, 0, 1]; //gravel
        textureValues[2] = aMap[0, 0, 2]; //dirt with stones
        textureValues[3] = aMap[0, 0, 3]; //stone
        textureValues[4] = aMap[0, 0, 4]; //dirt (wet)
        textureValues[5] = aMap[0, 0, 5]; //dirt (water)
    }
    private bool CheckTerrain()
    {
        Vector3 origin = transform.position + Vector3.up * 0.03f;
        Vector3 direction = (Vector3.down);
        direction.Normalize();
        float distance = 0.1f;
        Debug.DrawRay(origin, direction * distance, Color.yellow);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.CompareTag("Terrain")) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    void GetGroundMaterial()
    {
        Vector3 origin = transform.position + Vector3.up * 0.03f;
        Vector3 direction = (Vector3.down);
        direction.Normalize();
        float distance = 0.05f;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.transform.gameObject.CompareTag("Grass"))
            {
                groundTexture = 0;
            }
            else if (hit.transform.gameObject.CompareTag("Stone"))
            {
                groundTexture = 1;
            }
            else if (hit.transform.gameObject.CompareTag("Wood"))
            {
                groundTexture = 2;
            }
        }
    }
    #endregion

}
