using UnityEngine;

public class ColorCycle : MonoBehaviour
{
    public float speed = 0.2f;          // оборотов по кругу в секунду
    [Range(0, 1)] public float saturation = 1f;
    [Range(0, 1)] public float value = 1f;
    public string colorName = "_EmissionColor";//"_BaseColor";

    Material mat;
    float hue;

    public float intensity = 3f; // как в инспекторе

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION"); // без этого эмиссия может быть выключена
    }

    void Update()
    {
        hue = (hue + speed * Time.deltaTime) % 1f;
        Color c = Color.HSVToRGB(hue, saturation, value);
        mat.SetColor(colorName, c * Mathf.Pow(2f, intensity));
    }
}
