using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioBlock_Example1 : BioBlock
{

    public override void TakeStep()
    {
        base.TakeStep();

        if ((step > 21) && (step < 25))
        {
            SpawnNewBioBlock(Direction.Up);
        }
        else if (generation < 5)
        {
            SpawnNewBioBlock(Direction.Forward);
        }
        else if (generation < 10)
        {
            SpawnNewBioBlock(Direction.Left);
        }
        else if (generation < 15)
        {
            SpawnNewBioBlock(Direction.Back);
        }
        else if (generation < 20)
        {
            SpawnNewBioBlock(Direction.Right);
        }

    }

}
