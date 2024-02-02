using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

namespace PlanChess
{
    public class CodeStylingExample : MonoBehaviour //<--- Class Names are capital
    {
        public List<GameObject> ListOfSpaces; //<--- Capitals for public variables
        private bool _boolName; //<--- _underscore for private variables
    
        // Keep variables private, and only public if another system has to use it
        [HideInInspector] //<--- use this attribute to hide public variables inside the editor

    
        public void SomeFunction(int someInt)
        {   //<--- Brackets on New line 
            someInt = 0;// <--- scoped variables starts lower case
        }
    
        //Group ideas with new line
    
        //Leave comments in code to explain what a function does an
    
    
    
        /*
         -------------------------------------------------
         Example of Singleton - shouldnt be neccesary to use while having services
         ------------------------------------------------
     
        public static GameStateManager Instance;
        void Awake()
        {
             //Assign instance or destroy other instances
             if (Instance != null || Instance != this)
             {
                 Destroy(this);
             }
             else
             {
                 Instance = this;
             }
         }
     
         */

        //Working with services:
    
        private EasyService<GameStateManager> _gameStateManager; //<-- On Declared
        private GameStateManager __gameStateManager = ServiceLocator.GetService<GameStateManager>();  //<-- During runtime
    
        //Services are referenced using VALUE
        //e.g.
        void test()
        {
            GameState state = _gameStateManager.Value.GameState;
            // or
            _gameStateManager.Value.GameState = GameState.WIN;
        }
    
    }
}

