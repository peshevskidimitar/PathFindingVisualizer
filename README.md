# Визуелизација на пребарувачки алгоритми - Pathfinding Visualizer
Проект по Визуелно програмирање изработен од Димитар Пешевски и Данило Најков.
## Опис на апликацијата
Апликацијата *Pathfinding Visualizer* за цел има визуелен приказ на општопознати пребарувачки алгоритми како DFS, BFS, Greedy и A\* за откривање на пат од почетна до целна позиција во даден лавиринт. Но, пред да се започне со визуелизација на било кој пребарувачки алгоритам потребно е да се создаде лавиринтот којшто ќе се истражува, за што постојат две можности - лавиринтот да биде создаден целосно од страна на корисникот или да биде создаден случајно со помош на Примовиот алгоритам за генерирање на MST. По создавањето на лавиринтот се избираат почетната и целната позиција кои иницијално секогаш се поставени во горниот лев агол и долниот десен агол, соодветно. Откако лавиринтот ќе го добие својот конечен изглед со избор од dropdown листа се селектира посакуваниот алгоритам од пребарувачките алгоритми кои погоре беа споменати: DFS, BFS, Greedy и A*, а потоа соодветно се започнува со визуелизација на избраниот пребарувачки алгоритам преку притискање на копчето ```Run```. Во склоп на апликацијата постои можност за серијализација - зачувување (```Save``` / ```Save As```) и вчитување (```Open```) на состојбата на апликацијата, заедно со можноста за вчитување на нова иницијална состојба (```New```).
## Изглед на апликацијата
Корисничкиот интерфејс на апликацијата е дизајниран да биде едноставен и интуитивен. Во горниот дел од формата се наоѓа menu bar со опциите: ```New```, ```Open```, ```Save```, ```Save As``` и ```Exit``` групирани под опцијата ```File```. Веднаш под menu bar-от се наоѓа tool bar со опциите: ```New```, ```Open``` и ```Save``` претставени преку икони, додека десно од нив се наоѓаат опциите кои се однесуваат на генерирањето на лавиринтот. Корисникот има можност да избира помеѓу две опции за автоматско генерирање на лавиринтот ```Instant``` и ```Show algorithm``` групирани под опцијата ```Generate Random Maze```. Опцијата ```Instant``` овозможува едновремено генерирање на случаен лавиринт, додека опцијата ```Show algorithm``` го визуелизира процесот на генерирање на лавиринтот со помош на Примовиот алгоритам. Во склоп на опциите за конструкција на лавиринтот се и опциите ```Clear``` и ```Empty```, од коишто првата ја пребришува визуелизацијата на пребарувачкиот алгоритам, додека втората опција целосно го пребришува лавиринтот отстранувајќи ги сите пречки (obstacles) во него. Во главниот дел од прозорецот се наоѓаат уште две опции ```Size of a single cell``` којашто овозможува прилагодување на големината на ќелиите и ```Search algorithm``` којашто овозможува избор на пребарувачки алгоритам за визуелизација преку dropdown листа и негов приказ преку копчето ```Run```. Централно и најзначајно место во формата зафаќа приказот на самиот лавиринт чијашто големина може да се прилагодува со промена на големината на главната форма (resize). Во дното на формата се наоѓа status bar којшто после завршувањето на визуелизацијата на избраниот алгоритам прикажува колку ќелии биле истражени и колкава е должината на пронајдениот пат.

![1](https://user-images.githubusercontent.com/85986325/175771366-559aa535-3826-4e0f-8c13-01452798054b.png)
*Слика 1. „Приказ на основниот поглед на апликацијата“*
## Насоки за употреба на апликацијата
Во рамки на апликацијата се овозможени две главни фунцкионалности - *конструкција на лавиринт* и *визуелизација на пребарувачки алгоритми*.
Конструкцијата на лавиринтот може да биде автоматизирана преку опциите ```Instant``` и ```Show algorithm``` групирани под опцијата ```Generate Random Maze``` или пак мануелна, т.е. лавиринтот да биде создаден од страна на корисникот. За мануелно градење на лавиринтот постојат две функционалности:
- Поставување на пречка (obstacle) преку единечен лев клик на соодветната ќелија или пак со држење на левиот клик и поминување над ќелиите коишто корисникот смета дека треба да бидат означени како пречки.
- Отстранување на пречка (obstacle) преку единечен десен клик на соодветната ќелија или пак со држење на десниот клик и поминување над ќелиите коишто корисникот смета дека не треба да бидат означени како пречки.


Откако ќе биде завршен процесот на градење на лавиринтот корисникот, исто така може да ги помести почетната и целната позиција со помош на drag and drop. Конечно, по завршувањето на градењето на лавиринтот може да се избере посакуваниот алгоритам и да се отпочне неговата визуелизација.

![2](https://user-images.githubusercontent.com/85986325/175782422-ca2f7206-4b62-4f4f-90a6-089263dc4160.png)
*Слика 2. „Приказ на случајно генериран лавиринт“*

![3](https://user-images.githubusercontent.com/85986325/175782425-46a9d921-4992-4dfe-85b3-dccaad5eda85.png)
*Слика 3. „Приказ на рачно изграден лавиринт“*
## Интерна структура на апликацијата
Структурата на апликацијата е поделена во неколку класи и тоа:
- Класа *Scene* којашто претставува интерна репрзентација на документот што се користи за манипулација, зачувување и вчитување на состојбата на апликацијата.


```C#
[Serializable]
public class Scene
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
    public int Margin { get; set; }
    public int CellSize { get; set; }
    public Cell[, ] Cells { get; set; }

    public bool IsStartCellSelected { get; set; }
    public bool IsEndCellSelected { get; set; }
    
    public Scene(int width, int height);
    public void GenerateCells();
    public bool IsThereStartFlag();
    public bool IsThereEndFlag();
    public void ClearStartFlags();
    public void ClearEndFlags();
    public void UpdateSize(int width, int height);
    public void UpdateCellSize(int size);
    private void UpdateCells();
    public void Draw(Graphics graphics);
    public void Click(Point location, bool IsLeftClick, Point point);
    public bool IsStartCellClicked(Point location);
    public bool IsEndCellClicked(Point location);
    public void ClearVisitedAndPathFlags();
    public void BFS(Form form, ToolStripStatusLabel tssLblReport);
    public void DFS(Form form, ToolStripStatusLabel tssLblReport);
    public void Greedy(Form form, ToolStripStatusLabel tssLblReport);
    public void AStar(Form form, ToolStripStatusLabel tssLblReport);
    public void GenerateMaze(Form form, bool showAlgorithm);
}
```
- Класа *Cell* којашто претставува репрезентација на ќелиите на лавиринтот и ги содржи потребните методи за исцртување и манипулација со состојбата на ќелиите.

```C#
[Serializable]
public class Cell
{
    public enum State { Normal, Obstacle, Start, End }

    public Point TopLeft { get; set; }
    public Size Size { get; set; }
    public State PreviousState { get; set; }
    public State CurrentState { get; set; }
    public bool IsVisited { get; set; }
    public bool IsHighlighted { get; set; }
    public bool IsPath { get; set; }

    public Cell(Point topLeft, Size size);
    public void ChangeState(State newState);
    public void UndoState();
    private Color ChooseColor();
    public void Draw(Graphics graphics);
    public bool IsClicked(Point point);
}
```
- Класа *GraphNode* којашто претставува репрезентација на јазел во графовската репрезентација на лавиринтот.

```C#
public class GraphNode
{
    public int Index { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public Cell Cell { get; set; }
    public List<GraphNode> Neighbors { get; set; }

    public GraphNode Parent { get; set; }
    public float PathCost { get; set; }

    public GraphNode(int row, int column, int index, Cell cell);
    public bool ContainsNeighbor(GraphNode node);
    public void AddNeighbor(GraphNode node);
    public bool RemoveNeighbor(GraphNode node);
}
```
- Класа *Graph* којашто претставува графовска репрезентација на лавиринтот и во себе ги содржи имплементациите на пребарувачките алгоритми.

```C#
public class Graph
{
    private int CountOfNodes { get; set; }
    private List<GraphNode> AdjacencyList { get; set; }
    public GraphNode StartNode { get; set; }
    public bool FoundSolution { get; set; } = false;
    public int MaxRow { get; set; }
    public int MaxColumn { get; set; }

    private readonly Random rand = new Random();

    public Graph(Cell[,] cells);
    private void CheckNeighborhood(Cell[, ] cells, int i, int j, int x, int y);
    private void GenerateGraph(Cell[,] cells);
    private void FindStartNode();
    private void Wait(int time);
    private void RetrievePath(GraphNode end, Dictionary<int, int> parentNodes, Form form, ToolStripStatusLabel tssLblReport);
    public void GenerateRandomMaze(Form form, bool showAlgoritm);
    public void BFS(Form form, ToolStripStatusLabel tssLblReport);
    public void DFS(Form form, ToolStripStatusLabel tssLblReport);
    private void DFS_Recursive(GraphNode node, GraphNode parent, bool[] visited, Dictionary<int, int> parentNodes, Form form, int exploredNodes, ToolStripStatusLabel tssLblReport);
    public void Greedy(Form form, ToolStripStatusLabel tssLblReport, int CellSize);
    public void AStar(Form form, ToolStripStatusLabel tssLblReport, int CellSize);
    private double Heuristic(GraphNode startNode, GraphNode endNode, int CellSize);
}
```
## Серијализација на состојбата на апликацијата
Како дополнителна функционалност во склоп на апликацијата е имплементирана серијализација на состојбата, т.е. зачувување и вчитување на истата преку двете серијализирачки класи Scene и Cell. При секоја акција што се презема во рамки на апликацијата се води евиденција за промените и при исклучување на апликацијата или вчитување на нова/постоечка состојба се појавува прозорец којшто го прашува корисникот дали сака да ги зачува последните промени. Во продолжение се прикажани опциите за серијализација.

![4](https://user-images.githubusercontent.com/85986325/175782432-8dd1cc79-dbcc-4918-8ab8-d2be517f44f5.png)
*Слика 4. „Приказ на опциите за серијализација (1)“*

![5](https://user-images.githubusercontent.com/85986325/175782434-88952c4b-a32c-4eb5-bae3-24587f4453c3.png)
*Слика 5. „Приказ на опциите за серијализација (2)“*
## Алгоритми
### За пребарување
 - При секое поминување се означува полето со зелено. 
 ![image](https://user-images.githubusercontent.com/55097438/175505694-6927554f-3712-4cbf-81a6-ed2a2b033353.png)
 - При најден пат се означува патот со виолетово.
 ![image](https://user-images.githubusercontent.com/55097438/175505840-116e5c60-0446-44d4-85f2-f4094918eb34.png)
 
 #### BFS
 Се имплементира со редица
 #### DFS
 Се имплементира рекурзивно
 #### Greedy
 Се имплементира со приоритетна редица каде елементите се рангираат според менхетен растојание
 #### A*
 Се имплементира со приоритетна редица каде елементите се рангираат според збир на растојание и евристичка функција - менхетен растојание
 
### За генерирање на лавиринт
Се користи примов алгоритам за генерирање на MST. Има две опции:
- Инстант - веднаш се генерира нов лавиринт
- Прикажи алгоритам - се прикажува процесот на генерирање на лавиринт со овој алгоритам
