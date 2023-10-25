using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the base class of BioBlock

public class BioBlock : MonoBehaviour
{
    // you can set these in the Unity Inspector to customize your BioBlock
    public float stepDuration = 2f;
    public Color firstGenerationColor;
    public float darkeningFactor = 0.95f;  // the lower this number, the more the color of BioBlocks will darken with each generation

    // external variables BioBlocks can reference
    protected int step => BioBlockManager.Instance.step_count;

    // variables specific to this instance of BioBlock
    public int generation { get; set; }
    protected int birthstep;
    protected int age => (BioBlockManager.Instance.step_count - birthstep);
    protected Color myColor;

    private string computedName;
    public new string name
    {
        get
        {
            if (string.IsNullOrEmpty(computedName))
            {
                computedName = new string("BioBlock ");
                computedName = computedName + "(" + transform.position.x.ToString("F0") + "," + transform.position.y.ToString("F0") + "," + transform.position.z.ToString("F0") + ") ";
                computedName = computedName + "gen " + generation.ToString();
            }
            return computedName;
        }
    }

    private int computed_neighborCount;   // we store a computed neighbor count so that it never has to be generated twice in one step
    private int step_neighborCount_last_computed;
    public int neighborCount
    {
        get
        {
            if (step_neighborCount_last_computed < step)
                computed_neighborCount = BioBlockManager.Instance.CountNeighbors(this);

            return computed_neighborCount;
        }
    }

    // non-game-accessible variables specific to this instance of BioBlock - you can't access these directly
    public bool flag { get; set; }
    [HideInInspector] public Direction storedDirection = Direction.NONE;
    [HideInInspector] public bool traitActive;



    // override this method (and set the stepDuration variable) to create a custom bioblock
    public virtual void TakeStep ()
    {

    }


    // the section below is methods the bioblock designer can access

    protected void SpawnNewBioBlock (Direction direction)
    {
        BioBlockManager.Instance.SpawnNewBioBlock(this, direction, false);
    }
    protected void SpawnNewBioBlockWithFlag(Direction direction)
    {
        BioBlockManager.Instance.SpawnNewBioBlock(this, direction, true);
    }

    protected void RemoveThisBioBlock()
    {
        BioBlockManager.Instance.RemoveBioBlock(this, Direction.NONE);
    }
    protected void RemoveThisBioBlockInDirection(Direction direction)
    {
        BioBlockManager.Instance.RemoveBioBlock(this, direction);
    }

    protected void SetBioBlockFlag ()
    {
        flag = true;
    }
    protected void ClearBioBlockFlag()
    {
        flag = false;
    }
    protected bool CheckBioBlockFlag()
    {
        return flag;
    }
    protected void SetGlobalFlag()
    {
        BioBlockManager.Instance.globalFlag = true;
    }
    protected void ClearGlobalFlag()
    {
        BioBlockManager.Instance.globalFlag = false;
    }
    protected bool CheckGlobalFlag()
    {
        return BioBlockManager.Instance.globalFlag;
    }

    protected Direction GetStoredDirection ()
    {
        return storedDirection;
    }
    protected void StoreDirection (Direction direction)
    {
        storedDirection = direction;
    }

    protected Direction GetRandomDirection()
    {
        return WorldRep.GetRandomDirection();
    }

    protected Direction GetDirectionTowardsOrigin()
    {
        return BioBlockManager.Instance.GetDirectionTowardsOrigin(this);
    }
    protected Direction GetDirectionAwayFromOrigin()
    {
        return BioBlockManager.Instance.GetDirectionAwayFromOrigin(this);
    }

    protected float GetDistanceFromOrigin ()
    {
        return BioBlockManager.Instance.GetDistanceFromOrigin(this);
    }

    // returns the number of neighbors this BioBlock has
    protected int CountNeighbors()
    {
        return neighborCount;
    }

    protected bool NeighborCountAbove (int max)
    {
        return (neighborCount > max);
    }
    protected bool NeighborCountBelow(int min)
    {
        return (neighborCount < min);
    }
    protected bool NeighborCountWithinRange (int min_inclusive, int max_inclusive)
    {
        return ((neighborCount >= min_inclusive) && (neighborCount <= max_inclusive));
    }
    protected bool NeighborCountOutsideOfRange(int min_inclusive, int max_inclusive)
    {
        return !((neighborCount >= min_inclusive) && (neighborCount <= max_inclusive));
    }

    protected void ActivateTrait()
    {
        traitActive = true;
        UpdateAppearance();
    }
    protected void DeactivateTrait()
    {
        traitActive = false;
        UpdateAppearance();
    }

    // the section below is internal-only service methods


    // this is called once when a new BioBlock is spawned
    private void Start()
    {
        birthstep = step;

        myColor = firstGenerationColor;
        for (int i = 0; i < generation; i++)
            myColor = myColor * darkeningFactor;

        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        // Get the MeshRenderer component attached to the GameObject
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

        // Check if a MeshRenderer is found
        if (meshRenderer != null)
        {
            // Clone the material to avoid changing the shared material of all instances
            Material newMaterial = new Material(meshRenderer.sharedMaterial);

            // Set the new color
            if (!traitActive)
                newMaterial.color = myColor;
            else
                newMaterial.color = Color.white;

            // Assign the new material to the MeshRenderer
            meshRenderer.material = newMaterial;
        }
        else
        {
            Debug.LogError("MeshRenderer component not found on this GameObject.");
        }
    }
}
