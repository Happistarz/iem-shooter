using UnityEngine;

public class TractorBeamsController : MonoBehaviour
{
    public TractorBeamComponent[] tractorBeams;
    public TractorBeamComponent   bossBeam;

    private void Start()
    {
        Game.TractorBeamsController = this;
    }
    
    public int ActivateBossBeam()
    {
        bossBeam.gameObject.SetActive(true);
        bossBeam.FadeIn();
        return Mathf.CeilToInt(bossBeam.offsetSeconds + 1.2f);
    }

    public float ActivateBeams()
    {
        var maxDuration = 0f;
        MoveRandomBeams(0);
        
        foreach (var beam in tractorBeams)
        {
            beam.FadeIn();
            var duration = beam.offsetSeconds + 1.2f;
            if (duration > maxDuration) maxDuration = duration;
        }

        return maxDuration;
    }

    public void DeactivateBeams()
    {
        foreach (var beam in tractorBeams)
        {
            beam.FadeOut();
        }
    }
    
    public void MoveRandomBeams(int? index = null)
    {
        foreach (var beam in tractorBeams)
        {
            if (!beam.moveBetweenPositions) continue;
            beam.MoveTo(index ?? Random.Range(0, beam.positionMarkers.Length));
        }
    }
    
    public void ChangeBeamsColors()
    {
        foreach (var beam in tractorBeams)
        {
            beam.FadeToBossColor();
        }
        
        bossBeam.FadeToBossColor();
    }

    public SpawnLocationComponent GetRandomActiveBeamLocation()
    {
        var randomIndex = Random.Range(0, tractorBeams.Length);
        return tractorBeams[randomIndex].location;
    }
}