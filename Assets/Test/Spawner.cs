using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject _prefab;

    float randomValue { get { return Random.Range(0.75f, 1.25f); } }

    void Start()
    {
        for (var i = 0; i < 20; i++)
        {
            for (var j = 0; j < 20; j++)
            {
                var pos = new Vector3((i - 10) * 2, 0, (j - 10) * 2);
                var go = Instantiate(_prefab, pos, Quaternion.identity);
                var dancer = go.GetComponent<Puppet.Dancer>();

                dancer.stepFrequency      *= randomValue;
                dancer.stepHeight         *= randomValue;
                dancer.stepAngle          *= randomValue;
                dancer.hipHeight          *= randomValue;
                dancer.hipPositionNoise   *= randomValue;
                dancer.hipRotationNoise   *= randomValue;
                dancer.spineBend          *= randomValue;
                dancer.spineRotationNoise *= randomValue;
                dancer.headMove           *= randomValue;
                dancer.noiseFrequency     *= randomValue;

                dancer.footDistance *= Random.Range(0.8f, 2.0f);
                dancer.handPositionNoise *= Random.Range(0.8f, 2.0f);
                dancer.handPosition += Random.insideUnitSphere * 0.25f;

                dancer.randomSeed = Random.Range(0, 0xffffff);

                dancer.GetComponentInChildren<Renderer>().material.color =
                    Random.ColorHSV(0, 1, 0.6f, 1, 0.5f, 0.8f);
            }
        }
    }
}
