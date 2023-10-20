using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioBlockManager : MonoBehaviour
{
    public static BioBlockManager Instance;
    private UIManager ui;

    public static int maxNumBioBlocks = 499;  // the system won't spawn new BioBlocks if this number would be exceeded

    // variables that BioBlocks can reference
    public int step_count { get; private set; }
    public bool globalFlag;


    private float stepDuration;  // it slurps this from the first BioBlock
    private BioBlock firstBlock;  // points to firstBlock(ie - the block for whom generation == 0)
    private Vector3 firstBlockPosition;
    private List<BioBlock> allBlocks;
    private List<BioBlock> newBlocksToAdd;
    private List<BioBlock> blocksToRemove;
    public int totalBioBlocks => allBlocks.Count;

    private float timeNextStep;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError("Duplicate BioBlockManager detected, deleting it.");
            GameObject.Destroy(gameObject);
            return;
        }

        // TODO - check for multiple UIs
        ui = FindAnyObjectByType<UIManager>();
        if (ui == null)
            Debug.LogError("UI not found.  There must be excatly 1 UI in the scene.");

        firstBlock = GameObject.FindFirstObjectByType<BioBlock>();          // there should be exactly 1 bioblock in the world
        stepDuration = firstBlock.stepDuration;
        firstBlockPosition = firstBlock.transform.position;

        allBlocks = new List<BioBlock>();
        allBlocks.Add(firstBlock);

        step_count = 0;
        timeNextStep = Time.time + stepDuration;

        DoErrorChecking();
    }


    private void Update()
    {
        if (Time.time < timeNextStep)
            return;

        // it's time to take a step
        step_count++;

        // reset the lists
        newBlocksToAdd = new List<BioBlock>();
        blocksToRemove = new List<BioBlock>();

        // take the steps
        foreach (BioBlock block in allBlocks)
            block.TakeStep();

        // apply the changes scheduled when each block took a step
        foreach (BioBlock block in newBlocksToAdd)
            allBlocks.Add(block);
        foreach (BioBlock block in blocksToRemove)
        {
            allBlocks.Remove(block);
            GameObject.Destroy(block.gameObject);
        }

        // schedule the next step
        timeNextStep = Time.time + stepDuration;

        ui.UpdateUI();
    }


    // spawn a new bioblock in the space in passed-in direction relative to the BioBlock calling this method.
    // does nothing if there is already a BioBlock in that direction.
    public virtual void SpawnNewBioBlock(BioBlock caller, Direction direction, bool flag)
    {
        if (allBlocks.Count >= maxNumBioBlocks)  // add newBlocksToAdd.Count to allBlocks.Count to be strict about not exceeding the limit,  otherwise it is allowed durign a step
            return;

        Vector3 new_position = WorldRep.ApplyDirection(caller.transform.position, direction);

        // if a block already exists there or one has been scheduled to be added there, don't add one
        if ((FindBioBlockAtPosition(new_position) != null) || (FindBioBlockAtPosition(new_position, newBlocksToAdd) != null))
            return;

        GameObject new_bioblock_go = GameObject.Instantiate(caller.gameObject, new_position, transform.rotation);

        BioBlock new_bioblock = new_bioblock_go.GetComponent<BioBlock>();

        // set its data
        new_bioblock.generation = caller.generation + 1;
        new_bioblock.gameObject.name = new_bioblock.name;
        if (flag)
            new_bioblock.flag = true;
        new_bioblock.storedDirection = caller.storedDirection;
        new_bioblock.traitActive = caller.traitActive;

        newBlocksToAdd.Add(new_bioblock);
    }


    // removes the bioblock in the space in passed-in direction relative to the BioBLock calling this method.
    // does nothing if there is no BioBlock in that direction
    public virtual void RemoveBioBlock(BioBlock caller, Direction direction)
    {
        Vector3 target_position = WorldRep.ApplyDirection(caller.transform.position, direction);

        BioBlock target_bioblock = FindBioBlockAtPosition(target_position);

        if (target_bioblock != null)
            blocksToRemove.Add(target_bioblock);
    }


    private BioBlock FindBioBlockAtPosition(Vector3 position)
    {
        return FindBioBlockAtPosition(position, allBlocks);
    }
    private BioBlock FindBioBlockAtPosition(Vector3 position, List<BioBlock> listToSearch)
    {
        foreach (BioBlock block in listToSearch)
            if (WorldRep.EqualWithinTolerance(block.transform.position, position))
                return block;

        return null;
    }


    // returns the number of neighbors the passed-in bioblock has.
    // this version is more performant because it assumes (correctly) than any bioblock on the allBlocks list within 1.74 units is a neighbor (other than this same block)
    public int CountNeighbors (BioBlock block)
    {
        int count = 0;
        Vector3 block_pos = block.transform.position;  // copy it as an optimization

        foreach (BioBlock b in allBlocks)
            if (((block_pos - b.transform.position).sqrMagnitude) < 3.0276f) // 3.0276 is 1.74 ^ 2,   this is a performant way to check distances between 2 vector3s
                count++;

        return count-1;  // minus one because don't count yourself
    }


    // returns the number of neighbors the passed-in bioblock has.
    // this version is less peformant, but it actually checks each candidate block to make sure it is a neighbor (ie - it is on the grid and in one of the valid directions).  this seems to be unnecessary checking.
    public int CountNeighbors_MorePrecise (BioBlock block)
    {

        Vector3 target_position;
        int count = 0;

        // first round up a list of all bioblocks which are candidates
        // this is an optimization so we only have to iterate over the entire bioblock list once
        List<BioBlock> nearby_blocks = new List<BioBlock>();
        foreach (BioBlock b in allBlocks)
            // check to see if this bioblock is less than 1.74 units away.  if further away, it cannot possiby be a neighbor
            if (((block.transform.position - b.transform.position).sqrMagnitude) < 3.0276f) // 3.0276 is 1.74 ^ 2
                nearby_blocks.Add(b);

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            if (dir != Direction.NONE)  // don't count yourself
            {
                target_position = WorldRep.ApplyDirection(block.transform.position, dir);
                if (FindBioBlockAtPosition(target_position, nearby_blocks) != null)
                    count++;
            }
        }

        return count;
    }


    public Direction GetDirectionTowardsOrigin(BioBlock fromBlock)
    {
        Direction dir = WorldRep.MapVectorToDirection(firstBlockPosition - fromBlock.transform.position);
//        Debug.Log($"The direction from the origin block to {fromBlock.name} is {dir}");
        return dir;
    }
    public Direction GetDirectionAwayFromOrigin(BioBlock fromBlock)
    {
        Direction dir = WorldRep.MapVectorToDirection(fromBlock.transform.position - firstBlockPosition);
//        Debug.Log($"The direction from block {fromBlock.name} to the origin block is {dir}");
        return dir;
    }

    public float GetDistanceFromOrigin (BioBlock fromBlock)
    {
        Vector3 vector = fromBlock.transform.position - firstBlockPosition;
        return vector.magnitude;
    }

    private void DoErrorChecking()
    {
        // TODO - check all bioblocks in the scene, and for each make sure it only has 1 BioBlock derived script on it.
    }

}
