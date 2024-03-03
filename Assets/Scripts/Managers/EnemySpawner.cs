using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using Random = System.Random;

public class EnemySpawner : MonoService
{
    // Services
    private EasyService<GameStateManager> _gameStateManager;
    private EasyService<BoardManager> _boardManager;
    private IndexCode[][] _boardRows;
    
    // Settings
    public int MaxTurnsWithoutSpawning = 4;
    private int _lastTurnWithSpawns = 0;
    
    // Useful
    Random random = new Random();
    
    void Start()
    {
        InitializeRowData();
    }

    public void ExecuteSpawning()
    {
        // Variables
        int turnNumber = _gameStateManager.Value.GetTurnNumber();
        // Are there going to be spawns this turn?
        bool spawnThisTurn = GetSpawningBool(turnNumber);
        
        Debug.Log($"Executing Spawning. Turn number {turnNumber}");
        
        if (spawnThisTurn)
        {
            Array pieceValues = Enum.GetValues(typeof(ChessPieceType));
            ChessPieceType randomPiece = (ChessPieceType)pieceValues.GetValue(random.Next(pieceValues.Length));
            int rowForSpawning = random.Next(5, 7);
            int columnForSpawning = random.Next(0, _boardManager.Value.GetBoardDimensions()[0]);
            IndexCode position = _boardRows[rowForSpawning][columnForSpawning];

            Debug.Log($"Spawning enemy chess piece of type {randomPiece}, on IndexCode {position}");
            
            // Create chess piece. If the board square is already occupied, loop until a valid board square is found.
            ChessPiece piece = null;
            int counter = 0;
            while (piece == null)
            {
                counter++;
                piece = SpawnEnemy(randomPiece, position);
                
                // Relocate chess piece position if it is not valid
                rowForSpawning = random.Next(5, 7);
                columnForSpawning = random.Next(0, _boardManager.Value.GetBoardDimensions()[0]);
                position = _boardRows[rowForSpawning][columnForSpawning];
                
                // If all square boards are occupied, no spawning is done
                // This prevents an infinite loop if all board squares have their pieces assigned
                if (counter > 24)
                {
                    return;
                }
            }
            
            // Random upgrades
            double rangeUpgradeChance = random.NextDouble();
            double speedUpgradeChance = random.NextDouble();
            // Only upgrade pieces after turn 10
            if (turnNumber < 10)
            {
                // do nothing
            }
            else if (turnNumber < 25)
            {
                if (rangeUpgradeChance < 0.3)
                    piece.Range++;
                if (speedUpgradeChance < 0.5)
                    piece.Speed++;
            }
            else if (turnNumber < 50)
            {
                if (rangeUpgradeChance < 0.6)
                    piece.Range++;
                if (speedUpgradeChance < 0.5)
                    piece.Speed++;
            }
            else
            {
                // If turn is superior to 50, piece is upgraded by default
                piece.Range++;
                piece.Speed++;
                
                // Can be upgraded twice at this point
                if (rangeUpgradeChance > 0.5)
                    piece.Range++;
                if (speedUpgradeChance < 0.5)
                    piece.Speed++;
            }
        }

        // Once spawning is finished, transition to preparation phase
        _gameStateManager.Value.GameState = GameState.PREP;
    }

    // Generate an array of arrays with the position of the board squares
    private void InitializeRowData()
    {
        int rowCount = _boardManager.Value.GetBoardDimensions()[0];
        int columnCount = _boardManager.Value.GetBoardDimensions()[1];
        _boardRows =  new IndexCode[rowCount][];
        
        for (int row = 0; row < rowCount; row++)
        {
            int skipCounter = 0;
            IndexCode[] indexCodesPerRow = new IndexCode[columnCount];
            for (int column = 0; column < columnCount; column++)
            {
                indexCodesPerRow[column] = (IndexCode)(skipCounter + row);
                skipCounter += rowCount;
            }

            _boardRows[row] = indexCodesPerRow;
        }
        DebugRowIndexCodes();
    }

    
    // Debugs the rows in boardRows array. Helps to visualize if the rows are correctly setup
    private void DebugRowIndexCodes()
    {
        for (int i = 0; i < _boardManager.Value.GetBoardDimensions()[0]; i++)
        {
            string indexCodes = "";
            foreach (int intCode in _boardRows[i])
            {
                indexCodes += $" {(IndexCode)intCode} ";
            }
            Debug.Log($"Row {i} has the following index codes: [{indexCodes}]");
        }
    }

    // Returns how many enemy chess pieces will spawn this turn
    private bool GetSpawningBool(int turnNumber)
    {
        bool SpawnByTurnExpiration =
            _lastTurnWithSpawns + MaxTurnsWithoutSpawning < turnNumber;

        // Spawning by expiration should be disabled on the first 5 turns
        if (turnNumber < 5)
        {
            SpawnByTurnExpiration = false;
        }
        
        bool SpawnByChance = random.NextDouble() * 100 < GetSpawnChance(turnNumber);

        if (SpawnByTurnExpiration || SpawnByChance)
        {
            return true;
        }

        return false;
    }

    // Returns the spawn chance based on the turn. A mathematical formula is used
    // The spawn chance is higher, the higher the turn count is
    private double GetSpawnChance(int turnNumber)
    {
        double exponent = 1.5 / (15.0 / (turnNumber + 14));
        return 15.0f + Math.Pow(2.0, exponent);
    }

    private ChessPiece SpawnEnemy(ChessPieceType pieceType, IndexCode indexCode)
    {
        _lastTurnWithSpawns = _gameStateManager.Value.GetTurnNumber();

        BoardSquare tile = _boardManager.Value.GetTile(indexCode);
        BeamController beam = Instantiate(GlobalGameAssets.Instance.BeamVFXPrefab).GetComponent<BeamController>();
        beam.transform.position = tile.CenterSurfaceTransform.position;
        beam.Order = 7 - tile.IndexX + 1;
        beam.PlayVFX();

        return _boardManager.Value.CreatePiece(pieceType, indexCode, Team.Enemy);
    }
}
