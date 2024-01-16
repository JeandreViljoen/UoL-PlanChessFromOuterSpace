# UoL-PlanChessFromOuterSpace

Code style:
public class BoardManager : MonoBehaviour //<--- Class Names are capital
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
}
