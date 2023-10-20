using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// common classes and methods used for interacting with this game world

public enum Direction
{
    Forward,    // positive Z
    Back,       // negative Z
    Right,      // positive X
    Left,       // negative X
    Up,         // positive Y
    Down,       // negative Y
    ForwardRight,
    ForwardLeft,
    BackRight,
    BackLeft,
    UpFoward,
    UpBack,
    UpLeft,
    UpRight,
    UpForwardRight,
    UpForwardLeft,
    UpBackRight,
    UpBackLeft,
    DownForward,
    DownBack,
    DownLeft,
    DownRight,
    DownForwardRight,
    DownForwardLeft,
    DownBackRight,
    DownBackLeft,
    NONE
}


// TODO - this probably doesn't need to inherit from MonoBehavior

public class WorldRep : MonoBehaviour
{

    public static float EPSILON_position = 0.01f;    // two positions within this tolerance are considered to be at the same position


    public static bool EqualWithinTolerance (Vector3 positionA, Vector3 positionB)
    {
        return ((Mathf.Abs(positionA.x - positionB.x) < EPSILON_position) &&
            (Mathf.Abs(positionA.y - positionB.y) < EPSILON_position) &&
            (Mathf.Abs(positionA.z - positionB.z) < EPSILON_position));
    }


    // returns a random direction other than NONE
    public static Direction GetRandomDirection()
    {
        return (Direction)Random.Range(0, 26);
    }


    // returns a Vector3 position which is 1 unit in the passed-in direction relative to the start position.
    // for diagonals, it moves 1 unit in each orthagonal direction, eg - ForwardRight is 1 unit forward and 1 unit right.
    public static Vector3 ApplyDirection (Vector3 start, Direction direction)
    {
        return ApplyDirection(start, direction, 1f);
    }

    // returns a Vector3 position which is X units in the passed-in direction relative to the start position, where X is the passed-in amount.
    // for diagonals, it moves X units in each orthagonal direction, eg - ForwardRight is X units forward and X units right.
    public static Vector3 ApplyDirection(Vector3 start, Direction direction, float amount)
    {
        Vector3 toReturn = new Vector3(start.x, start.y, start.z);

        switch (direction)
        {
            case Direction.Forward:
                toReturn.z += amount;
                break;

            case Direction.Back:
                toReturn.z -= amount;
                break;

            case Direction.Left:
                toReturn.x -= amount;
                break;

            case Direction.Right:
                toReturn.x += amount;
                break;

            case Direction.Up:
                toReturn.y += amount;
                break;

            case Direction.Down:
                toReturn.y -= amount;
                break;

            case Direction.ForwardRight:
                toReturn.z += amount;
                toReturn.x += amount;
                break;

            case Direction.ForwardLeft:
                toReturn.z += amount;
                toReturn.x -= amount;
                break;

            case Direction.BackRight:
                toReturn.z -= amount;
                toReturn.x += amount;
                break;

            case Direction.BackLeft:
                toReturn.z -= amount;
                toReturn.x -= amount;
                break;

            case Direction.UpFoward:
                toReturn.y += amount;
                toReturn.z += amount;
                break;

            case Direction.UpBack:
                toReturn.y += amount;
                toReturn.z -= amount;
                break;

            case Direction.UpLeft:
                toReturn.y += amount;
                toReturn.x -= amount;
                break;

            case Direction.UpRight:
                toReturn.y += amount;
                toReturn.x += amount;
                break;

            case Direction.UpForwardRight:
                toReturn.y += amount;
                toReturn.z += amount;
                toReturn.x += amount;
                break;

            case Direction.UpForwardLeft:
                toReturn.y += amount;
                toReturn.z += amount;
                toReturn.x -= amount;
                break;

            case Direction.UpBackRight:
                toReturn.y += amount;
                toReturn.z -= amount;
                toReturn.x += amount;
                break;

            case Direction.UpBackLeft:
                toReturn.y += amount;
                toReturn.z -= amount;
                toReturn.x -= amount;
                break;

            case Direction.DownForward:
                toReturn.y -= amount;
                toReturn.z += amount;
                break;

            case Direction.DownBack:
                toReturn.y -= amount;
                toReturn.z -= amount;
                break;

            case Direction.DownLeft:
                toReturn.y -= amount;
                toReturn.x -= amount;
                break;

            case Direction.DownRight:
                toReturn.y -= amount;
                toReturn.x += amount;
                break;

            case Direction.DownForwardRight:
                toReturn.y -= amount;
                toReturn.z += amount;
                toReturn.x += amount;
                break;

            case Direction.DownForwardLeft:
                toReturn.y -= amount;
                toReturn.z += amount;
                toReturn.x -= amount;
                break;

            case Direction.DownBackRight:
                toReturn.y -= amount;
                toReturn.z -= amount;
                toReturn.x += amount;
                break;

            case Direction.DownBackLeft:
                toReturn.y -= amount;
                toReturn.z -= amount;
                toReturn.x -= amount;
                break;
        }

        return toReturn;
    }


    // returns the Direction that the passed-in vector most closely aligns with.
    // TODO - this kind of works okay, but it's definitely not precise/correct for all cases
    public static Direction MapVectorToDirection(Vector3 vector)
    {
        // Normalize the input vector to ensure it has a magnitude of 1
        vector.Normalize();

        // Calculate the dot product of the vector with each direction
        float dotUp = Vector3.Dot(vector, Vector3.up);
        float dotDown = Vector3.Dot(vector, Vector3.down);
        float dotForward = Vector3.Dot(vector, Vector3.forward);
        float dotBack = Vector3.Dot(vector, Vector3.back);
        float dotRight = Vector3.Dot(vector, Vector3.right);
        float dotLeft = Vector3.Dot(vector, Vector3.left);

        float threshold = 0.577f; 

        bool forward = dotForward > threshold;
        bool back = dotBack > threshold;
        bool up = dotUp > threshold;
        bool down = dotDown > threshold;
        bool right = dotRight > threshold;
        bool left = dotLeft > threshold;

        // Determine the direction based on dot products
        if (up && forward && right)
        {
            return Direction.UpForwardRight;
        }
        else if (up && forward && left)
        {
            return Direction.UpForwardLeft;
        }
        else if (up && back && right)
        {
            return Direction.UpBackRight;
        }
        else if (up && back && left)
        {
            return Direction.UpBackLeft;
        }
        else if (down && forward && right)
        {
            return Direction.DownForwardRight;
        }
        else if (down && forward && left)
        {
            return Direction.DownForwardLeft;
        }
        else if (down && back && right)
        {
            return Direction.DownBackRight;
        }
        else if (down && back && left)
        {
            return Direction.DownBackLeft;
        }
        else if (forward && right)
        {
            return Direction.ForwardRight;
        }
        else if (forward && left)
        {
            return Direction.ForwardLeft;
        }
        else if (back && right)
        {
            return Direction.BackRight;
        }
        else if (back && left)
        {
            return Direction.BackLeft;
        }
        else if (forward && up)
        {
            return Direction.UpFoward;
        }
        else if (forward && down)
        {
            return Direction.DownForward;
        }
        else if (back && up)
        {
            return Direction.UpBack;
        }
        else if (back && down)
        {
            return Direction.DownBack;
        }
        else if (right && up)
        {
            return Direction.UpRight;
        }
        else if (right && down)
        {
            return Direction.DownRight;
        }
        else if (left && up)
        {
            return Direction.UpLeft;
        }
        else if (left && down)
        {
            return Direction.DownLeft;
        }
        else if (up)
        {
            return Direction.Up;
        }
        else if (down)
        {
            return Direction.Down;
        }
        else if (forward)
        {
            return Direction.Forward;
        }
        else if (back)
        {
            return Direction.Back;
        }
        else if (right)
        {
            return Direction.Right;
        }
        else if (left)
        {
            return Direction.Left;
        }
        else
        {
            return Direction.NONE;
        }

    }


}
