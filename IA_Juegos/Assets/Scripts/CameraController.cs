using UnityEngine;
using UnityEngine.UI;

/**
 * @class CameraController
 * @brief Controlador de las cámaras de la escena.
 */
public class CameraController : MonoBehaviour
{
    private Camera _mainCamera;             // Cámara principal
    private Camera _influenceCamera;        // Cámara del mapa de influencia
    private RenderTexture[] _textures;      // Texturas de los minimapas
    private RawImage _miniMap;              // Minimapa en la escena
    private int _index;                     // Índice del mapa de influencia a mostrar

    private GameObject _mainButtonsUI;      // Objeto de los botones con el mapa principal
    private GameObject _influenceButtonsUI; // Objeto de los botones con el mapa de influencia

    private GridController _gridController;

    private void Awake()
    {
        // Obtenemos los objetos
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _influenceCamera = GameObject.Find("Influence Camera").GetComponent<Camera>();
        _miniMap = GameObject.Find("Minimap").GetComponent<RawImage>();
        _mainButtonsUI = GameObject.Find("MainButtonsUI");
        _influenceButtonsUI = GameObject.Find("InfluenceButtonsUI");

        // Texturas para el minimapa
        _textures = new RenderTexture[2];
        _textures[0] = Resources.Load<RenderTexture>("miniMap");
        _textures[1] = Resources.Load<RenderTexture>("InfluenceTexture");
        
        _index = 1;
        _mainCamera.enabled = true;
        _mainButtonsUI.SetActive(true);
        _influenceCamera.enabled = false;
        _influenceButtonsUI.SetActive(false);
        _miniMap.texture = _textures[_index];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchInfluenceView();
        }
    }

    // Intercambia la cámara principal y la de la del mapa de influencia
    private void SwitchInfluenceView()
    {
        _mainCamera.enabled = !_mainCamera.enabled;
        _mainButtonsUI.SetActive(!_mainButtonsUI.activeSelf);
        _influenceCamera.enabled = !_influenceCamera.enabled;
        _influenceButtonsUI.SetActive(!_influenceButtonsUI.activeSelf);
        _index = (_index + 1) % _textures.Length;
        _miniMap.texture = _textures[_index];
    }
}
