using Dreamteck.Splines;
using UnityEngine;

public class SplineFollowerAttach : MonoBehaviour
{
    private SplineFollower splineFollower;
    private SplineProjector splineProjector;
    private PlayerMovement playerMovement;

    private SplineComputer currentSplineComputer;

    private bool following = false;

    private void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        splineProjector = GetComponent<SplineProjector>();
        playerMovement = GetComponent<PlayerMovement>();
        splineFollower.onEndReached += SplineFollower_onEndReached;
    }

    private void SplineFollower_onEndReached(double obj)
    {
        playerMovement.ExitSpline();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("WaterPlataform"))
        {
            currentSplineComputer = collision.collider.GetComponent<SplineComputer>();
            splineFollower.spline = currentSplineComputer;
            splineProjector.spline = currentSplineComputer;
            EnterSpline();
        }
    }

    public void ClearSplineComputer()
    {
        Destroy(currentSplineComputer.gameObject, 0.2f);
        currentSplineComputer = null;
        splineFollower.spline = null;
        splineProjector.spline = null;
    }

    public void EnterSpline()
    {
        if (currentSplineComputer == null) return;
        following = true;
        splineFollower.follow = true;

        CalculateProjection();
    }

    public void ExitSpline(bool forced = false)
    {
        if (currentSplineComputer == null) return;
        if (!following && !forced) return;
        following = false;
        splineFollower.follow = false;

        ClearSplineComputer();
    }

    private void CalculateProjection()
    {
        SplineSample sample = splineProjector.result;
        splineFollower.SetPercent(sample.percent);
    }
}
