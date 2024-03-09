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
    private EasyService<AudioManager> _audioManager;
    private IndexCode[][] _boardRows;
    
    // Settings
    public int InitialNumberOfSpawns = 6;
    
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

        // Initial spawning
        if (turnNumber == 1)
        {
            SpawnInitialPieces();
        }
        else
        {
            // Get number of spawns
            int spawnsThisTurn = GetSpawningUnitNumber(turnNumber);
        
            Debug.Log($"Executing Spawning. Turn number {turnNumber}");
            if (spawnsThisTurn > 0)
            {
                for (int i = 0; i < spawnsThisTurn; i++)
                {
                    bool spawned = SpawnRandomlyOnBoard(true);
                    if (!spawned)
                    {
                        Debug.LogError("Error loading initial spawns!");
                    }
                }
            }
        }
        
        // Spawning Sound
        _audioManager.Value.PlaySound(Sound.Lightning, this.gameObject);

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

    // Initial Spawns
    public void SpawnInitialPieces()
    {
        Debug.Log($"Executing starting spawning. {InitialNumberOfSpawns} pieces will be spawned");
        for (int i = 0; i < InitialNumberOfSpawns; i++)
        {
            SpawnRandomlyOnBoard(false);
        }
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
    private int GetSpawningUnitNumber(int turnNumber)
    {
        // Always spawn at least one unit
        int numberOfSpawns = 1;
        
        // 30% Chance for a second spawn
        if (random.NextDouble() * 100 < 30)
            numberOfSpawns++;

        // Third spawn chance is based on a quadratic function that increases with the turn number
        if (random.NextDouble() * 100 < GetSpawnChance(turnNumber))
            numberOfSpawns++;

        return numberOfSpawns;
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
        BoardSquare tile = _boardManager.Value.GetTile(indexCode);
        BeamController beam = Instantiate(GlobalGameAssets.Instance.BeamVFXPrefab).GetComponent<BeamController>();
        beam.transform.position = tile.CenterSurfaceTransform.position;
        beam.Order = 7 - tile.IndexX + 1;
        beam.PlayVFX();

        return _boardManager.Value.CreatePiece(pieceType, indexCode, Team.Enemy);
    }

    private ChessPieceType GetRandomSpawningPiece()
    {
        // Get an array of all the ChessPiece types
        Array pieceValues = Enum.GetValues(typeof(ChessPieceType));
        ChessPieceType pieceType = (ChessPieceType)pieceValues.GetValue(random.Next(pieceValues.Length));

        // If piece type is king, iterate until other ChessPieceType is found
        while (pieceType == ChessPieceType.King)
        {
            pieceType = (ChessPieceType)pieceValues.GetValue(random.Next(pieceValues.Length));
        }

        return pieceType;
    }

    private bool SpawnRandomlyOnBoard(bool enableUpgrades)
    {
        int turnNumber = _gameStateManager.Value.GetTurnNumber();
        ChessPieceType randomPiece = GetRandomSpawningPiece();
        int rowForSpawning = random.Next(5, 7);
        int columnForSpawning = random.Next(0, _boardManager.Value.GetBoardDimensions()[0]);
        IndexCode position = _boardRows[rowForSpawning][columnForSpawning];
                
        // Create chess piece. If the board square is already occupied, loop until a valid board square is found.
        ChessPiece piece = null;
        int counter = 0;
        while (piece == null)
        {
            counter++;
            piece = SpawnEnemy(randomPiece, position);

            if (piece)
            {
                Debug.Log($"Spawning enemy chess piece of type {randomPiece}, on IndexCode {position}");
                // Avoid ChessPïece functions division by zero
                piece.Range = 1;
                piece.Speed = 1;
                break;
            }
                    
            // Relocate chess piece position if it is not valid
            rowForSpawning = random.Next(5, 7);
            columnForSpawning = random.Next(0, _boardManager.Value.GetBoardDimensions()[0]);
            position = _boardRows[rowForSpawning][columnForSpawning];
                    
            // If all square boards are occupied, no spawning is done
            // This prevents an infinite loop if all board squares have their pieces assigned
            if (counter > 24)
            {
                // If no squares are available, do not spawn
                Debug.Log("Enemy could not be created because there is not a valid board square.");
                return false;
            }
        }
        
        if (piece && enableUpgrades)
        {
            Debug.Log("Applying upgrades to spawned piece.");
            // -- Random upgrades
            // Only upgrade pieces after turn 3
            if (turnNumber < 3)
            {
                // do nothing
            }
            else if (turnNumber < 10)
            {
                // Increase speed and range by 0 or 1
                RandomlyUpgradePiece(piece, 1, 1);
            }
            else if (turnNumber < 15)
            {
                RandomlyUpgradePiece(piece, 2, 2);
            }
            else if (turnNumber < 20)
            {
                RandomlyUpgradePiece(piece, 3, 3);
            }
            else
            {
                // If turn is superior to 20, pieces are upgraded by default
                piece.Range++;
                piece.Speed++;
                    
                // Can be upgraded twice at this point
                RandomlyUpgradePiece(piece, 3, 3);
            } 
        }

        return true;
    }

    private void RandomlyUpgradePiece(ChessPiece chessPiece, int maximumRange, int maximumSpeed)
    {
        int rangeUpgrade = random.Next(0, maximumRange);
        int speedUpgrade = random.Next(0, maximumSpeed);

        chessPiece.Range += rangeUpgrade;
        chessPiece.Speed += speedUpgrade;
    }
}
