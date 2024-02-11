using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Services;

public class RelativeMovesets : MonoBehaviour
{ 
  public MovesetData MovesetDataObject;
    
  // Class initialization
  private void Start()
  {
      try
      {
          string relativeMovesetsString = File.ReadAllText(Application.dataPath +
                                                           "/Scripts/JSON/relative_movesets.json");
          SetupMovesetData(relativeMovesetsString);
      }
      catch (Exception ex)
      {
          Debug.Log("Movesets could not be generated. Exception: ${ex}");
      }
  }

  // Reads JSON and serializes into MovesetData object
  private void SetupMovesetData(string jsonString)
  {
      MovesetDataObject = JsonUtility.FromJson<MovesetData>(jsonString);
  }
  
  // Gets an specific moveset
  public List<Vector2> GetChessPieceMoveset(Team team, string pieceType)
  {
      TeamBehavior mTeamBehavior = MovesetDataObject.TeamBehaviors.Find((t) => t.Behavior == (int)team);
      if (mTeamBehavior != null)
      {
          ChessPieceMovement pieceMovementData =
              mTeamBehavior.ChessPieces.Find((movement) => movement.ChessPieceName == pieceType);
          if (pieceMovementData != null)
              return pieceMovementData.Movesets;
      }
      
      Debug.Log("ChessPieceMovement could not be retrieved from the moveset data!");
      return new List<Vector2>();
  }
}



// -- Serializable classes for loading JSON onto a MovesetData object --
[Serializable]
public class MovesetData
{
    public List<TeamBehavior> TeamBehaviors;
}

[Serializable]
public class TeamBehavior
{
    public int Behavior;
    public List<ChessPieceMovement> ChessPieces;
}

[Serializable]
public class ChessPieceMovement
{
    public string ChessPieceName;
    public List<Vector2> Movesets;
}