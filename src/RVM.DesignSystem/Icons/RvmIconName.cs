namespace RVM.DesignSystem.Icons;

/// <summary>
/// Os icones disponiveis. Enum, e nao string: nome de icone errado vira erro de compilacao em vez
/// de um quadrado vazio na tela.
/// </summary>
/// <remarks>
/// Desenhos do conjunto Tabler (MIT, <c>Icons/LICENSE-tabler.txt</c>), copiados para dentro do pacote — sem CDN e
/// sem dependencia de runtime (ADR-005). Os primeiros 46 tem o nome do Tabler; os demais sao os nomes do
/// <c>RvmIconName</c> do RVM.UI (Solar), exigidos pelo contrato de API (DSGN-017), cada um desenhado com o Tabler
/// de significado mais proximo — mapa em <c>tools/icones-do-contrato.json</c>.
/// </remarks>
public enum RvmIconName
{
    /// <summary>Icone <c>AlertCircle</c> do conjunto Tabler.</summary>
    AlertCircle,

    /// <summary>Icone <c>AlertTriangle</c> do conjunto Tabler.</summary>
    AlertTriangle,

    /// <summary>Icone <c>ArrowLeft</c> do conjunto Tabler.</summary>
    ArrowLeft,

    /// <summary>Icone <c>ArrowRight</c> do conjunto Tabler.</summary>
    ArrowRight,

    /// <summary>Icone <c>ArrowUp</c> do conjunto Tabler.</summary>
    ArrowUp,

    /// <summary>Icone <c>ArrowDown</c> do conjunto Tabler.</summary>
    ArrowDown,

    /// <summary>Icone <c>Bell</c> do conjunto Tabler.</summary>
    Bell,

    /// <summary>Icone <c>Calendar</c> do conjunto Tabler.</summary>
    Calendar,

    /// <summary>Icone <c>Check</c> do conjunto Tabler.</summary>
    Check,

    /// <summary>Icone <c>ChevronDown</c> do conjunto Tabler.</summary>
    ChevronDown,

    /// <summary>Icone <c>ChevronLeft</c> do conjunto Tabler.</summary>
    ChevronLeft,

    /// <summary>Icone <c>ChevronRight</c> do conjunto Tabler.</summary>
    ChevronRight,

    /// <summary>Icone <c>ChevronUp</c> do conjunto Tabler.</summary>
    ChevronUp,

    /// <summary>Icone <c>CircleCheck</c> do conjunto Tabler.</summary>
    CircleCheck,

    /// <summary>Icone <c>Clock</c> do conjunto Tabler.</summary>
    Clock,

    /// <summary>Icone <c>Close</c> do conjunto Tabler.</summary>
    Close,

    /// <summary>Icone <c>Copy</c> do conjunto Tabler.</summary>
    Copy,

    /// <summary>Icone <c>DotsVertical</c> do conjunto Tabler.</summary>
    DotsVertical,

    /// <summary>Icone <c>Download</c> do conjunto Tabler.</summary>
    Download,

    /// <summary>Icone <c>ExternalLink</c> do conjunto Tabler.</summary>
    ExternalLink,

    /// <summary>Icone <c>Eye</c> do conjunto Tabler.</summary>
    Eye,

    /// <summary>Icone <c>EyeOff</c> do conjunto Tabler.</summary>
    EyeOff,

    /// <summary>Icone <c>File</c> do conjunto Tabler.</summary>
    File,

    /// <summary>Icone <c>Filter</c> do conjunto Tabler.</summary>
    Filter,

    /// <summary>Icone <c>Heart</c> do conjunto Tabler.</summary>
    Heart,

    /// <summary>Icone <c>Home</c> do conjunto Tabler.</summary>
    Home,

    /// <summary>Icone <c>InfoCircle</c> do conjunto Tabler.</summary>
    InfoCircle,

    /// <summary>Icone <c>Loader</c> do conjunto Tabler.</summary>
    Loader,

    /// <summary>Icone <c>Lock</c> do conjunto Tabler.</summary>
    Lock,

    /// <summary>Icone <c>Logout</c> do conjunto Tabler.</summary>
    Logout,

    /// <summary>Icone <c>Mail</c> do conjunto Tabler.</summary>
    Mail,

    /// <summary>Icone <c>Menu</c> do conjunto Tabler.</summary>
    Menu,

    /// <summary>Icone <c>Minus</c> do conjunto Tabler.</summary>
    Minus,

    /// <summary>Icone <c>Pencil</c> do conjunto Tabler.</summary>
    Pencil,

    /// <summary>Icone <c>Phone</c> do conjunto Tabler.</summary>
    Phone,

    /// <summary>Icone <c>Plus</c> do conjunto Tabler.</summary>
    Plus,

    /// <summary>Icone <c>Printer</c> do conjunto Tabler.</summary>
    Printer,

    /// <summary>Icone <c>Refresh</c> do conjunto Tabler.</summary>
    Refresh,

    /// <summary>Icone <c>Search</c> do conjunto Tabler.</summary>
    Search,

    /// <summary>Icone <c>Settings</c> do conjunto Tabler.</summary>
    Settings,

    /// <summary>Icone <c>Star</c> do conjunto Tabler.</summary>
    Star,

    /// <summary>Icone <c>Table</c> do conjunto Tabler.</summary>
    Table,

    /// <summary>Icone <c>Trash</c> do conjunto Tabler.</summary>
    Trash,

    /// <summary>Icone <c>Upload</c> do conjunto Tabler.</summary>
    Upload,

    /// <summary>Icone <c>User</c> do conjunto Tabler.</summary>
    User,

    /// <summary>Icone <c>Users</c> do conjunto Tabler.</summary>
    Users,

    // --- Gerado por tools/icones-do-contrato.py a partir daqui: nao editar a mao. ---

    /// <summary>Icone <c>Accessibility</c> do contrato com o RVM.UI, desenhado com o <c>accessible</c> do Tabler.</summary>
    Accessibility,

    /// <summary>Icone <c>Accumulator</c> do contrato com o RVM.UI, desenhado com o <c>battery-automotive</c> do Tabler.</summary>
    Accumulator,

    /// <summary>Icone <c>Add</c> do contrato com o RVM.UI, desenhado com o <c>plus</c> do Tabler.</summary>
    Add,

    /// <summary>Icone <c>AddCircle</c> do contrato com o RVM.UI, desenhado com o <c>circle-plus</c> do Tabler.</summary>
    AddCircle,

    /// <summary>Icone <c>AddSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-plus</c> do Tabler.</summary>
    AddSquare,

    /// <summary>Icone <c>AdhesivePlaster</c> do contrato com o RVM.UI, desenhado com o <c>bandage</c> do Tabler.</summary>
    AdhesivePlaster,

    /// <summary>Icone <c>AdhesivePlaster2</c> do contrato com o RVM.UI, desenhado com o <c>bandage</c> do Tabler.</summary>
    AdhesivePlaster2,

    /// <summary>Icone <c>Airbuds</c> do contrato com o RVM.UI, desenhado com o <c>ear</c> do Tabler.</summary>
    Airbuds,

    /// <summary>Icone <c>AirbudsCase</c> do contrato com o RVM.UI, desenhado com o <c>box</c> do Tabler.</summary>
    AirbudsCase,

    /// <summary>Icone <c>AirbudsCaseCharge</c> do contrato com o RVM.UI, desenhado com o <c>battery-charging</c> do Tabler.</summary>
    AirbudsCaseCharge,

    /// <summary>Icone <c>AirbudsCaseMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>box</c> do Tabler.</summary>
    AirbudsCaseMinimalistic,

    /// <summary>Icone <c>AirbudsCaseOpen</c> do contrato com o RVM.UI, desenhado com o <c>package-export</c> do Tabler.</summary>
    AirbudsCaseOpen,

    /// <summary>Icone <c>AirbudsCharge</c> do contrato com o RVM.UI, desenhado com o <c>battery-charging</c> do Tabler.</summary>
    AirbudsCharge,

    /// <summary>Icone <c>AirbudsCheck</c> do contrato com o RVM.UI, desenhado com o <c>ear</c> do Tabler.</summary>
    AirbudsCheck,

    /// <summary>Icone <c>AirbudsLeft</c> do contrato com o RVM.UI, desenhado com o <c>ear</c> do Tabler.</summary>
    AirbudsLeft,

    /// <summary>Icone <c>AirbudsRemove</c> do contrato com o RVM.UI, desenhado com o <c>ear-off</c> do Tabler.</summary>
    AirbudsRemove,

    /// <summary>Icone <c>AirbudsRight</c> do contrato com o RVM.UI, desenhado com o <c>ear</c> do Tabler.</summary>
    AirbudsRight,

    /// <summary>Icone <c>Alarm</c> do contrato com o RVM.UI, desenhado com o <c>alarm</c> do Tabler.</summary>
    Alarm,

    /// <summary>Icone <c>AlarmAdd</c> do contrato com o RVM.UI, desenhado com o <c>alarm-plus</c> do Tabler.</summary>
    AlarmAdd,

    /// <summary>Icone <c>AlarmPause</c> do contrato com o RVM.UI, desenhado com o <c>alarm</c> do Tabler.</summary>
    AlarmPause,

    /// <summary>Icone <c>AlarmPlay</c> do contrato com o RVM.UI, desenhado com o <c>alarm</c> do Tabler.</summary>
    AlarmPlay,

    /// <summary>Icone <c>AlarmRemove</c> do contrato com o RVM.UI, desenhado com o <c>alarm-minus</c> do Tabler.</summary>
    AlarmRemove,

    /// <summary>Icone <c>AlarmSleep</c> do contrato com o RVM.UI, desenhado com o <c>zzz</c> do Tabler.</summary>
    AlarmSleep,

    /// <summary>Icone <c>AlarmTurnOff</c> do contrato com o RVM.UI, desenhado com o <c>alarm-off</c> do Tabler.</summary>
    AlarmTurnOff,

    /// <summary>Icone <c>Album</c> do contrato com o RVM.UI, desenhado com o <c>album</c> do Tabler.</summary>
    Album,

    /// <summary>Icone <c>AlignBottom</c> do contrato com o RVM.UI, desenhado com o <c>align-box-bottom-center</c> do Tabler.</summary>
    AlignBottom,

    /// <summary>Icone <c>AlignHorizontaSpacing</c> do contrato com o RVM.UI, desenhado com o <c>layout-distribute-vertical</c> do Tabler.</summary>
    AlignHorizontaSpacing,

    /// <summary>Icone <c>AlignHorizontalCenter</c> do contrato com o RVM.UI, desenhado com o <c>align-box-center-middle</c> do Tabler.</summary>
    AlignHorizontalCenter,

    /// <summary>Icone <c>AlignLeft</c> do contrato com o RVM.UI, desenhado com o <c>align-left</c> do Tabler.</summary>
    AlignLeft,

    /// <summary>Icone <c>AlignRight</c> do contrato com o RVM.UI, desenhado com o <c>align-right</c> do Tabler.</summary>
    AlignRight,

    /// <summary>Icone <c>AlignTop</c> do contrato com o RVM.UI, desenhado com o <c>align-box-top-center</c> do Tabler.</summary>
    AlignTop,

    /// <summary>Icone <c>AlignVerticalCenter</c> do contrato com o RVM.UI, desenhado com o <c>align-box-center-middle</c> do Tabler.</summary>
    AlignVerticalCenter,

    /// <summary>Icone <c>AlignVerticalSpacing</c> do contrato com o RVM.UI, desenhado com o <c>layout-distribute-horizontal</c> do Tabler.</summary>
    AlignVerticalSpacing,

    /// <summary>Icone <c>AltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>chevron-down</c> do Tabler.</summary>
    AltArrowDown,

    /// <summary>Icone <c>AltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>chevron-left</c> do Tabler.</summary>
    AltArrowLeft,

    /// <summary>Icone <c>AltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>chevron-right</c> do Tabler.</summary>
    AltArrowRight,

    /// <summary>Icone <c>AltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>chevron-up</c> do Tabler.</summary>
    AltArrowUp,

    /// <summary>Icone <c>Archive</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    Archive,

    /// <summary>Icone <c>ArchiveDown</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    ArchiveDown,

    /// <summary>Icone <c>ArchiveDownNotes</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    ArchiveDownNotes,

    /// <summary>Icone <c>ArchiveUp</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    ArchiveUp,

    /// <summary>Icone <c>ArchiveUpNotes</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    ArchiveUpNotes,

    /// <summary>Icone <c>Archived</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    Archived,

    /// <summary>Icone <c>ArchivedMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    ArchivedMinimalistic,

    /// <summary>Icone <c>Armchair</c> do contrato com o RVM.UI, desenhado com o <c>armchair</c> do Tabler.</summary>
    Armchair,

    /// <summary>Icone <c>Armchair2</c> do contrato com o RVM.UI, desenhado com o <c>armchair-2</c> do Tabler.</summary>
    Armchair2,

    /// <summary>Icone <c>ArrowLeftDown</c> do contrato com o RVM.UI, desenhado com o <c>arrow-down-left</c> do Tabler.</summary>
    ArrowLeftDown,

    /// <summary>Icone <c>ArrowLeftUp</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-left</c> do Tabler.</summary>
    ArrowLeftUp,

    /// <summary>Icone <c>ArrowRightDown</c> do contrato com o RVM.UI, desenhado com o <c>arrow-down-right</c> do Tabler.</summary>
    ArrowRightDown,

    /// <summary>Icone <c>ArrowRightUp</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-right</c> do Tabler.</summary>
    ArrowRightUp,

    /// <summary>Icone <c>ArrowToDownLeft</c> do contrato com o RVM.UI, desenhado com o <c>arrow-down-left</c> do Tabler.</summary>
    ArrowToDownLeft,

    /// <summary>Icone <c>ArrowToDownRight</c> do contrato com o RVM.UI, desenhado com o <c>arrow-down-right</c> do Tabler.</summary>
    ArrowToDownRight,

    /// <summary>Icone <c>ArrowToTopLeft</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-left</c> do Tabler.</summary>
    ArrowToTopLeft,

    /// <summary>Icone <c>ArrowToTopRight</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-right</c> do Tabler.</summary>
    ArrowToTopRight,

    /// <summary>Icone <c>Asteroid</c> do contrato com o RVM.UI, desenhado com o <c>meteor</c> do Tabler.</summary>
    Asteroid,

    /// <summary>Icone <c>Atom</c> do contrato com o RVM.UI, desenhado com o <c>atom</c> do Tabler.</summary>
    Atom,

    /// <summary>Icone <c>AugmentedReality</c> do contrato com o RVM.UI, desenhado com o <c>augmented-reality</c> do Tabler.</summary>
    AugmentedReality,

    /// <summary>Icone <c>Backpack</c> do contrato com o RVM.UI, desenhado com o <c>backpack</c> do Tabler.</summary>
    Backpack,

    /// <summary>Icone <c>Backspace</c> do contrato com o RVM.UI, desenhado com o <c>backspace</c> do Tabler.</summary>
    Backspace,

    /// <summary>Icone <c>Bacteria</c> do contrato com o RVM.UI, desenhado com o <c>virus</c> do Tabler.</summary>
    Bacteria,

    /// <summary>Icone <c>Bag</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    Bag,

    /// <summary>Icone <c>Bag2</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    Bag2,

    /// <summary>Icone <c>Bag3</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    Bag3,

    /// <summary>Icone <c>Bag4</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    Bag4,

    /// <summary>Icone <c>Bag5</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    Bag5,

    /// <summary>Icone <c>BagCheck</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag-check</c> do Tabler.</summary>
    BagCheck,

    /// <summary>Icone <c>BagCross</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag-x</c> do Tabler.</summary>
    BagCross,

    /// <summary>Icone <c>BagHeart</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag-heart</c> do Tabler.</summary>
    BagHeart,

    /// <summary>Icone <c>BagMusic</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    BagMusic,

    /// <summary>Icone <c>BagMusic2</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    BagMusic2,

    /// <summary>Icone <c>BagSmile</c> do contrato com o RVM.UI, desenhado com o <c>shopping-bag</c> do Tabler.</summary>
    BagSmile,

    /// <summary>Icone <c>Balloon</c> do contrato com o RVM.UI, desenhado com o <c>balloon</c> do Tabler.</summary>
    Balloon,

    /// <summary>Icone <c>Balls</c> do contrato com o RVM.UI, desenhado com o <c>ball-bowling</c> do Tabler.</summary>
    Balls,

    /// <summary>Icone <c>Banknote</c> do contrato com o RVM.UI, desenhado com o <c>cash-banknote</c> do Tabler.</summary>
    Banknote,

    /// <summary>Icone <c>Banknote2</c> do contrato com o RVM.UI, desenhado com o <c>cash</c> do Tabler.</summary>
    Banknote2,

    /// <summary>Icone <c>BarChair</c> do contrato com o RVM.UI, desenhado com o <c>armchair</c> do Tabler.</summary>
    BarChair,

    /// <summary>Icone <c>Basketball</c> do contrato com o RVM.UI, desenhado com o <c>ball-basketball</c> do Tabler.</summary>
    Basketball,

    /// <summary>Icone <c>Bath</c> do contrato com o RVM.UI, desenhado com o <c>bath</c> do Tabler.</summary>
    Bath,

    /// <summary>Icone <c>BatteryCharge</c> do contrato com o RVM.UI, desenhado com o <c>battery-charging</c> do Tabler.</summary>
    BatteryCharge,

    /// <summary>Icone <c>BatteryChargeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>battery-charging</c> do Tabler.</summary>
    BatteryChargeMinimalistic,

    /// <summary>Icone <c>BatteryFull</c> do contrato com o RVM.UI, desenhado com o <c>battery-4</c> do Tabler.</summary>
    BatteryFull,

    /// <summary>Icone <c>BatteryFullMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>battery-4</c> do Tabler.</summary>
    BatteryFullMinimalistic,

    /// <summary>Icone <c>BatteryHalf</c> do contrato com o RVM.UI, desenhado com o <c>battery-2</c> do Tabler.</summary>
    BatteryHalf,

    /// <summary>Icone <c>BatteryHalfMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>battery-2</c> do Tabler.</summary>
    BatteryHalfMinimalistic,

    /// <summary>Icone <c>BatteryLow</c> do contrato com o RVM.UI, desenhado com o <c>battery-1</c> do Tabler.</summary>
    BatteryLow,

    /// <summary>Icone <c>BatteryLowMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>battery-1</c> do Tabler.</summary>
    BatteryLowMinimalistic,

    /// <summary>Icone <c>Bed</c> do contrato com o RVM.UI, desenhado com o <c>bed</c> do Tabler.</summary>
    Bed,

    /// <summary>Icone <c>BedEssentionalUi</c> do contrato com o RVM.UI, desenhado com o <c>bed</c> do Tabler.</summary>
    BedEssentionalUi,

    /// <summary>Icone <c>BedsideTable</c> do contrato com o RVM.UI, desenhado com o <c>table</c> do Tabler.</summary>
    BedsideTable,

    /// <summary>Icone <c>BedsideTable2</c> do contrato com o RVM.UI, desenhado com o <c>table</c> do Tabler.</summary>
    BedsideTable2,

    /// <summary>Icone <c>BedsideTable3</c> do contrato com o RVM.UI, desenhado com o <c>table</c> do Tabler.</summary>
    BedsideTable3,

    /// <summary>Icone <c>BedsideTable4</c> do contrato com o RVM.UI, desenhado com o <c>table</c> do Tabler.</summary>
    BedsideTable4,

    /// <summary>Icone <c>BellBing</c> do contrato com o RVM.UI, desenhado com o <c>bell-ringing</c> do Tabler.</summary>
    BellBing,

    /// <summary>Icone <c>BellOff</c> do contrato com o RVM.UI, desenhado com o <c>bell-off</c> do Tabler.</summary>
    BellOff,

    /// <summary>Icone <c>BenzeneRing</c> do contrato com o RVM.UI, desenhado com o <c>hexagon</c> do Tabler.</summary>
    BenzeneRing,

    /// <summary>Icone <c>Bicycling</c> do contrato com o RVM.UI, desenhado com o <c>bike</c> do Tabler.</summary>
    Bicycling,

    /// <summary>Icone <c>BicyclingRound</c> do contrato com o RVM.UI, desenhado com o <c>bike</c> do Tabler.</summary>
    BicyclingRound,

    /// <summary>Icone <c>Bill</c> do contrato com o RVM.UI, desenhado com o <c>receipt</c> do Tabler.</summary>
    Bill,

    /// <summary>Icone <c>BillCheck</c> do contrato com o RVM.UI, desenhado com o <c>receipt</c> do Tabler.</summary>
    BillCheck,

    /// <summary>Icone <c>BillCross</c> do contrato com o RVM.UI, desenhado com o <c>receipt-off</c> do Tabler.</summary>
    BillCross,

    /// <summary>Icone <c>BillList</c> do contrato com o RVM.UI, desenhado com o <c>receipt</c> do Tabler.</summary>
    BillList,

    /// <summary>Icone <c>BillList2</c> do contrato com o RVM.UI, desenhado com o <c>receipt</c> do Tabler.</summary>
    BillList2,

    /// <summary>Icone <c>BlackHole</c> do contrato com o RVM.UI, desenhado com o <c>circle-dotted</c> do Tabler.</summary>
    BlackHole,

    /// <summary>Icone <c>BlackHole2</c> do contrato com o RVM.UI, desenhado com o <c>circle-dotted</c> do Tabler.</summary>
    BlackHole2,

    /// <summary>Icone <c>BlackHole3</c> do contrato com o RVM.UI, desenhado com o <c>circle-dotted</c> do Tabler.</summary>
    BlackHole3,

    /// <summary>Icone <c>Bluetooth</c> do contrato com o RVM.UI, desenhado com o <c>bluetooth</c> do Tabler.</summary>
    Bluetooth,

    /// <summary>Icone <c>BluetoothCircle</c> do contrato com o RVM.UI, desenhado com o <c>bluetooth</c> do Tabler.</summary>
    BluetoothCircle,

    /// <summary>Icone <c>BluetoothSquare</c> do contrato com o RVM.UI, desenhado com o <c>bluetooth</c> do Tabler.</summary>
    BluetoothSquare,

    /// <summary>Icone <c>BluetoothWave</c> do contrato com o RVM.UI, desenhado com o <c>bluetooth-connected</c> do Tabler.</summary>
    BluetoothWave,

    /// <summary>Icone <c>Body</c> do contrato com o RVM.UI, desenhado com o <c>user</c> do Tabler.</summary>
    Body,

    /// <summary>Icone <c>BodyShape</c> do contrato com o RVM.UI, desenhado com o <c>user</c> do Tabler.</summary>
    BodyShape,

    /// <summary>Icone <c>BodyShapeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>user</c> do Tabler.</summary>
    BodyShapeMinimalistic,

    /// <summary>Icone <c>Bolt</c> do contrato com o RVM.UI, desenhado com o <c>bolt</c> do Tabler.</summary>
    Bolt,

    /// <summary>Icone <c>BoltCircle</c> do contrato com o RVM.UI, desenhado com o <c>bolt</c> do Tabler.</summary>
    BoltCircle,

    /// <summary>Icone <c>Bomb</c> do contrato com o RVM.UI, desenhado com o <c>bomb</c> do Tabler.</summary>
    Bomb,

    /// <summary>Icone <c>Bomb2</c> do contrato com o RVM.UI, desenhado com o <c>bomb</c> do Tabler.</summary>
    Bomb2,

    /// <summary>Icone <c>BombMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>bomb</c> do Tabler.</summary>
    BombMinimalistic,

    /// <summary>Icone <c>Bone</c> do contrato com o RVM.UI, desenhado com o <c>bone</c> do Tabler.</summary>
    Bone,

    /// <summary>Icone <c>BoneBroken</c> do contrato com o RVM.UI, desenhado com o <c>bone</c> do Tabler.</summary>
    BoneBroken,

    /// <summary>Icone <c>BoneCrack</c> do contrato com o RVM.UI, desenhado com o <c>bone</c> do Tabler.</summary>
    BoneCrack,

    /// <summary>Icone <c>Bones</c> do contrato com o RVM.UI, desenhado com o <c>bone</c> do Tabler.</summary>
    Bones,

    /// <summary>Icone <c>Bonfire</c> do contrato com o RVM.UI, desenhado com o <c>flame</c> do Tabler.</summary>
    Bonfire,

    /// <summary>Icone <c>Book</c> do contrato com o RVM.UI, desenhado com o <c>book</c> do Tabler.</summary>
    Book,

    /// <summary>Icone <c>Book2</c> do contrato com o RVM.UI, desenhado com o <c>book-2</c> do Tabler.</summary>
    Book2,

    /// <summary>Icone <c>BookBookmark</c> do contrato com o RVM.UI, desenhado com o <c>book</c> do Tabler.</summary>
    BookBookmark,

    /// <summary>Icone <c>BookBookmarkMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>book</c> do Tabler.</summary>
    BookBookmarkMinimalistic,

    /// <summary>Icone <c>BookMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>book</c> do Tabler.</summary>
    BookMinimalistic,

    /// <summary>Icone <c>Bookmark</c> do contrato com o RVM.UI, desenhado com o <c>bookmark</c> do Tabler.</summary>
    Bookmark,

    /// <summary>Icone <c>BookmarkCircle</c> do contrato com o RVM.UI, desenhado com o <c>bookmark</c> do Tabler.</summary>
    BookmarkCircle,

    /// <summary>Icone <c>BookmarkOpened</c> do contrato com o RVM.UI, desenhado com o <c>bookmark</c> do Tabler.</summary>
    BookmarkOpened,

    /// <summary>Icone <c>BookmarkSquare</c> do contrato com o RVM.UI, desenhado com o <c>bookmark</c> do Tabler.</summary>
    BookmarkSquare,

    /// <summary>Icone <c>BookmarkSquareMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>bookmark</c> do Tabler.</summary>
    BookmarkSquareMinimalistic,

    /// <summary>Icone <c>Boombox</c> do contrato com o RVM.UI, desenhado com o <c>speakerphone</c> do Tabler.</summary>
    Boombox,

    /// <summary>Icone <c>Bottle</c> do contrato com o RVM.UI, desenhado com o <c>bottle</c> do Tabler.</summary>
    Bottle,

    /// <summary>Icone <c>Bowling</c> do contrato com o RVM.UI, desenhado com o <c>bowling</c> do Tabler.</summary>
    Bowling,

    /// <summary>Icone <c>Box</c> do contrato com o RVM.UI, desenhado com o <c>box</c> do Tabler.</summary>
    Box,

    /// <summary>Icone <c>BoxMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>box</c> do Tabler.</summary>
    BoxMinimalistic,

    /// <summary>Icone <c>BranchingPathsDown</c> do contrato com o RVM.UI, desenhado com o <c>arrows-split</c> do Tabler.</summary>
    BranchingPathsDown,

    /// <summary>Icone <c>BranchingPathsUp</c> do contrato com o RVM.UI, desenhado com o <c>arrows-split</c> do Tabler.</summary>
    BranchingPathsUp,

    /// <summary>Icone <c>Broom</c> do contrato com o RVM.UI, desenhado com o <c>brush</c> do Tabler.</summary>
    Broom,

    /// <summary>Icone <c>Bug</c> do contrato com o RVM.UI, desenhado com o <c>bug</c> do Tabler.</summary>
    Bug,

    /// <summary>Icone <c>BugMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>bug</c> do Tabler.</summary>
    BugMinimalistic,

    /// <summary>Icone <c>Buildings</c> do contrato com o RVM.UI, desenhado com o <c>buildings</c> do Tabler.</summary>
    Buildings,

    /// <summary>Icone <c>Buildings2</c> do contrato com o RVM.UI, desenhado com o <c>building-skyscraper</c> do Tabler.</summary>
    Buildings2,

    /// <summary>Icone <c>Buildings3</c> do contrato com o RVM.UI, desenhado com o <c>buildings</c> do Tabler.</summary>
    Buildings3,

    /// <summary>Icone <c>Bus</c> do contrato com o RVM.UI, desenhado com o <c>bus</c> do Tabler.</summary>
    Bus,

    /// <summary>Icone <c>Calculator</c> do contrato com o RVM.UI, desenhado com o <c>calculator</c> do Tabler.</summary>
    Calculator,

    /// <summary>Icone <c>CalculatorMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>calculator</c> do Tabler.</summary>
    CalculatorMinimalistic,

    /// <summary>Icone <c>CalendarAdd</c> do contrato com o RVM.UI, desenhado com o <c>calendar-plus</c> do Tabler.</summary>
    CalendarAdd,

    /// <summary>Icone <c>CalendarDate</c> do contrato com o RVM.UI, desenhado com o <c>calendar</c> do Tabler.</summary>
    CalendarDate,

    /// <summary>Icone <c>CalendarMark</c> do contrato com o RVM.UI, desenhado com o <c>calendar-event</c> do Tabler.</summary>
    CalendarMark,

    /// <summary>Icone <c>CalendarMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>calendar</c> do Tabler.</summary>
    CalendarMinimalistic,

    /// <summary>Icone <c>CalendarSearch</c> do contrato com o RVM.UI, desenhado com o <c>calendar-search</c> do Tabler.</summary>
    CalendarSearch,

    /// <summary>Icone <c>CallCancel</c> do contrato com o RVM.UI, desenhado com o <c>phone-off</c> do Tabler.</summary>
    CallCancel,

    /// <summary>Icone <c>CallCancelRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-off</c> do Tabler.</summary>
    CallCancelRounded,

    /// <summary>Icone <c>CallChat</c> do contrato com o RVM.UI, desenhado com o <c>phone-call</c> do Tabler.</summary>
    CallChat,

    /// <summary>Icone <c>CallChatRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-call</c> do Tabler.</summary>
    CallChatRounded,

    /// <summary>Icone <c>CallDropped</c> do contrato com o RVM.UI, desenhado com o <c>phone-x</c> do Tabler.</summary>
    CallDropped,

    /// <summary>Icone <c>CallDroppedRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-x</c> do Tabler.</summary>
    CallDroppedRounded,

    /// <summary>Icone <c>CallMedicine</c> do contrato com o RVM.UI, desenhado com o <c>phone</c> do Tabler.</summary>
    CallMedicine,

    /// <summary>Icone <c>CallMedicineRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone</c> do Tabler.</summary>
    CallMedicineRounded,

    /// <summary>Icone <c>Camera</c> do contrato com o RVM.UI, desenhado com o <c>camera</c> do Tabler.</summary>
    Camera,

    /// <summary>Icone <c>CameraAdd</c> do contrato com o RVM.UI, desenhado com o <c>camera-plus</c> do Tabler.</summary>
    CameraAdd,

    /// <summary>Icone <c>CameraMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>camera</c> do Tabler.</summary>
    CameraMinimalistic,

    /// <summary>Icone <c>CameraRotate</c> do contrato com o RVM.UI, desenhado com o <c>camera-rotate</c> do Tabler.</summary>
    CameraRotate,

    /// <summary>Icone <c>CameraSquare</c> do contrato com o RVM.UI, desenhado com o <c>camera</c> do Tabler.</summary>
    CameraSquare,

    /// <summary>Icone <c>Card</c> do contrato com o RVM.UI, desenhado com o <c>credit-card</c> do Tabler.</summary>
    Card,

    /// <summary>Icone <c>Card2</c> do contrato com o RVM.UI, desenhado com o <c>credit-card</c> do Tabler.</summary>
    Card2,

    /// <summary>Icone <c>CardRecive</c> do contrato com o RVM.UI, desenhado com o <c>credit-card-pay</c> do Tabler.</summary>
    CardRecive,

    /// <summary>Icone <c>CardSearch</c> do contrato com o RVM.UI, desenhado com o <c>credit-card</c> do Tabler.</summary>
    CardSearch,

    /// <summary>Icone <c>CardSend</c> do contrato com o RVM.UI, desenhado com o <c>credit-card-pay</c> do Tabler.</summary>
    CardSend,

    /// <summary>Icone <c>CardTransfer</c> do contrato com o RVM.UI, desenhado com o <c>credit-card-refund</c> do Tabler.</summary>
    CardTransfer,

    /// <summary>Icone <c>Cardholder</c> do contrato com o RVM.UI, desenhado com o <c>id</c> do Tabler.</summary>
    Cardholder,

    /// <summary>Icone <c>Cart</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    Cart,

    /// <summary>Icone <c>Cart2</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    Cart2,

    /// <summary>Icone <c>Cart3</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    Cart3,

    /// <summary>Icone <c>Cart4</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    Cart4,

    /// <summary>Icone <c>Cart5</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    Cart5,

    /// <summary>Icone <c>CartCheck</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart-check</c> do Tabler.</summary>
    CartCheck,

    /// <summary>Icone <c>CartCross</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart-x</c> do Tabler.</summary>
    CartCross,

    /// <summary>Icone <c>CartLarge</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    CartLarge,

    /// <summary>Icone <c>CartLarge2</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    CartLarge2,

    /// <summary>Icone <c>CartLarge3</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    CartLarge3,

    /// <summary>Icone <c>CartLarge4</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    CartLarge4,

    /// <summary>Icone <c>CartLargeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart</c> do Tabler.</summary>
    CartLargeMinimalistic,

    /// <summary>Icone <c>CartPlus</c> do contrato com o RVM.UI, desenhado com o <c>shopping-cart-plus</c> do Tabler.</summary>
    CartPlus,

    /// <summary>Icone <c>Case</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    Case,

    /// <summary>Icone <c>CaseMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    CaseMinimalistic,

    /// <summary>Icone <c>CaseRound</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    CaseRound,

    /// <summary>Icone <c>CaseRoundMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    CaseRoundMinimalistic,

    /// <summary>Icone <c>CashOut</c> do contrato com o RVM.UI, desenhado com o <c>cash</c> do Tabler.</summary>
    CashOut,

    /// <summary>Icone <c>Cassette</c> do contrato com o RVM.UI, desenhado com o <c>device-audio-tape</c> do Tabler.</summary>
    Cassette,

    /// <summary>Icone <c>Cassette2</c> do contrato com o RVM.UI, desenhado com o <c>device-audio-tape</c> do Tabler.</summary>
    Cassette2,

    /// <summary>Icone <c>Cat</c> do contrato com o RVM.UI, desenhado com o <c>cat</c> do Tabler.</summary>
    Cat,

    /// <summary>Icone <c>Chair</c> do contrato com o RVM.UI, desenhado com o <c>armchair</c> do Tabler.</summary>
    Chair,

    /// <summary>Icone <c>Chair2</c> do contrato com o RVM.UI, desenhado com o <c>armchair</c> do Tabler.</summary>
    Chair2,

    /// <summary>Icone <c>Chandelier</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    Chandelier,

    /// <summary>Icone <c>Chart</c> do contrato com o RVM.UI, desenhado com o <c>chart-bar</c> do Tabler.</summary>
    Chart,

    /// <summary>Icone <c>Chart2</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    Chart2,

    /// <summary>Icone <c>ChartSquare</c> do contrato com o RVM.UI, desenhado com o <c>chart-bar</c> do Tabler.</summary>
    ChartSquare,

    /// <summary>Icone <c>ChatDots</c> do contrato com o RVM.UI, desenhado com o <c>message-dots</c> do Tabler.</summary>
    ChatDots,

    /// <summary>Icone <c>ChatLine</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    ChatLine,

    /// <summary>Icone <c>ChatRound</c> do contrato com o RVM.UI, desenhado com o <c>message-circle</c> do Tabler.</summary>
    ChatRound,

    /// <summary>Icone <c>ChatRoundCall</c> do contrato com o RVM.UI, desenhado com o <c>message-circle</c> do Tabler.</summary>
    ChatRoundCall,

    /// <summary>Icone <c>ChatRoundCheck</c> do contrato com o RVM.UI, desenhado com o <c>message-circle-check</c> do Tabler.</summary>
    ChatRoundCheck,

    /// <summary>Icone <c>ChatRoundDots</c> do contrato com o RVM.UI, desenhado com o <c>message-circle</c> do Tabler.</summary>
    ChatRoundDots,

    /// <summary>Icone <c>ChatRoundLike</c> do contrato com o RVM.UI, desenhado com o <c>message-circle-heart</c> do Tabler.</summary>
    ChatRoundLike,

    /// <summary>Icone <c>ChatRoundLine</c> do contrato com o RVM.UI, desenhado com o <c>message-circle</c> do Tabler.</summary>
    ChatRoundLine,

    /// <summary>Icone <c>ChatRoundMoney</c> do contrato com o RVM.UI, desenhado com o <c>message-circle-dollar</c> do Tabler.</summary>
    ChatRoundMoney,

    /// <summary>Icone <c>ChatRoundUnread</c> do contrato com o RVM.UI, desenhado com o <c>message-circle-exclamation</c> do Tabler.</summary>
    ChatRoundUnread,

    /// <summary>Icone <c>ChatRoundVideo</c> do contrato com o RVM.UI, desenhado com o <c>message-circle</c> do Tabler.</summary>
    ChatRoundVideo,

    /// <summary>Icone <c>ChatSquare</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    ChatSquare,

    /// <summary>Icone <c>ChatSquare2</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    ChatSquare2,

    /// <summary>Icone <c>ChatSquareArrow</c> do contrato com o RVM.UI, desenhado com o <c>message-forward</c> do Tabler.</summary>
    ChatSquareArrow,

    /// <summary>Icone <c>ChatSquareCall</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    ChatSquareCall,

    /// <summary>Icone <c>ChatSquareCheck</c> do contrato com o RVM.UI, desenhado com o <c>message-check</c> do Tabler.</summary>
    ChatSquareCheck,

    /// <summary>Icone <c>ChatSquareCode</c> do contrato com o RVM.UI, desenhado com o <c>message-code</c> do Tabler.</summary>
    ChatSquareCode,

    /// <summary>Icone <c>ChatSquareLike</c> do contrato com o RVM.UI, desenhado com o <c>message-heart</c> do Tabler.</summary>
    ChatSquareLike,

    /// <summary>Icone <c>ChatUnread</c> do contrato com o RVM.UI, desenhado com o <c>message-exclamation</c> do Tabler.</summary>
    ChatUnread,

    /// <summary>Icone <c>CheckCircle</c> do contrato com o RVM.UI, desenhado com o <c>circle-check</c> do Tabler.</summary>
    CheckCircle,

    /// <summary>Icone <c>CheckRead</c> do contrato com o RVM.UI, desenhado com o <c>checks</c> do Tabler.</summary>
    CheckRead,

    /// <summary>Icone <c>CheckSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-check</c> do Tabler.</summary>
    CheckSquare,

    /// <summary>Icone <c>Checklist</c> do contrato com o RVM.UI, desenhado com o <c>checklist</c> do Tabler.</summary>
    Checklist,

    /// <summary>Icone <c>ChecklistMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>checklist</c> do Tabler.</summary>
    ChecklistMinimalistic,

    /// <summary>Icone <c>ChefHatHeart</c> do contrato com o RVM.UI, desenhado com o <c>chef-hat</c> do Tabler.</summary>
    ChefHatHeart,

    /// <summary>Icone <c>ChefHatMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>chef-hat</c> do Tabler.</summary>
    ChefHatMinimalistic,

    /// <summary>Icone <c>ChefHet</c> do contrato com o RVM.UI, desenhado com o <c>chef-hat</c> do Tabler.</summary>
    ChefHet,

    /// <summary>Icone <c>CircleBottomDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-down</c> do Tabler.</summary>
    CircleBottomDown,

    /// <summary>Icone <c>CircleBottomUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-up</c> do Tabler.</summary>
    CircleBottomUp,

    /// <summary>Icone <c>CircleTopDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-down</c> do Tabler.</summary>
    CircleTopDown,

    /// <summary>Icone <c>CircleTopUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-up</c> do Tabler.</summary>
    CircleTopUp,

    /// <summary>Icone <c>City</c> do contrato com o RVM.UI, desenhado com o <c>building-community</c> do Tabler.</summary>
    City,

    /// <summary>Icone <c>Clapperboard</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    Clapperboard,

    /// <summary>Icone <c>ClapperboardEdit</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    ClapperboardEdit,

    /// <summary>Icone <c>ClapperboardOpen</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    ClapperboardOpen,

    /// <summary>Icone <c>ClapperboardOpenPlay</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    ClapperboardOpenPlay,

    /// <summary>Icone <c>ClapperboardPlay</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    ClapperboardPlay,

    /// <summary>Icone <c>ClapperboardText</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    ClapperboardText,

    /// <summary>Icone <c>Clipboard</c> do contrato com o RVM.UI, desenhado com o <c>clipboard</c> do Tabler.</summary>
    Clipboard,

    /// <summary>Icone <c>ClipboardAdd</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-plus</c> do Tabler.</summary>
    ClipboardAdd,

    /// <summary>Icone <c>ClipboardCheck</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-check</c> do Tabler.</summary>
    ClipboardCheck,

    /// <summary>Icone <c>ClipboardHeart</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-heart</c> do Tabler.</summary>
    ClipboardHeart,

    /// <summary>Icone <c>ClipboardList</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-list</c> do Tabler.</summary>
    ClipboardList,

    /// <summary>Icone <c>ClipboardRemove</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-x</c> do Tabler.</summary>
    ClipboardRemove,

    /// <summary>Icone <c>ClipboardText</c> do contrato com o RVM.UI, desenhado com o <c>clipboard-text</c> do Tabler.</summary>
    ClipboardText,

    /// <summary>Icone <c>ClockCircle</c> do contrato com o RVM.UI, desenhado com o <c>clock</c> do Tabler.</summary>
    ClockCircle,

    /// <summary>Icone <c>ClockSquare</c> do contrato com o RVM.UI, desenhado com o <c>clock</c> do Tabler.</summary>
    ClockSquare,

    /// <summary>Icone <c>CloseCircle</c> do contrato com o RVM.UI, desenhado com o <c>circle-x</c> do Tabler.</summary>
    CloseCircle,

    /// <summary>Icone <c>CloseSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-x</c> do Tabler.</summary>
    CloseSquare,

    /// <summary>Icone <c>Closet</c> do contrato com o RVM.UI, desenhado com o <c>door</c> do Tabler.</summary>
    Closet,

    /// <summary>Icone <c>Closet2</c> do contrato com o RVM.UI, desenhado com o <c>door</c> do Tabler.</summary>
    Closet2,

    /// <summary>Icone <c>Cloud</c> do contrato com o RVM.UI, desenhado com o <c>cloud</c> do Tabler.</summary>
    Cloud,

    /// <summary>Icone <c>CloudBolt</c> do contrato com o RVM.UI, desenhado com o <c>cloud-bolt</c> do Tabler.</summary>
    CloudBolt,

    /// <summary>Icone <c>CloudBoltMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>cloud-bolt</c> do Tabler.</summary>
    CloudBoltMinimalistic,

    /// <summary>Icone <c>CloudCheck</c> do contrato com o RVM.UI, desenhado com o <c>cloud-check</c> do Tabler.</summary>
    CloudCheck,

    /// <summary>Icone <c>CloudDownload</c> do contrato com o RVM.UI, desenhado com o <c>cloud-download</c> do Tabler.</summary>
    CloudDownload,

    /// <summary>Icone <c>CloudFile</c> do contrato com o RVM.UI, desenhado com o <c>cloud-up</c> do Tabler.</summary>
    CloudFile,

    /// <summary>Icone <c>CloudMinus</c> do contrato com o RVM.UI, desenhado com o <c>cloud-minus</c> do Tabler.</summary>
    CloudMinus,

    /// <summary>Icone <c>CloudPlus</c> do contrato com o RVM.UI, desenhado com o <c>cloud-plus</c> do Tabler.</summary>
    CloudPlus,

    /// <summary>Icone <c>CloudRain</c> do contrato com o RVM.UI, desenhado com o <c>cloud-rain</c> do Tabler.</summary>
    CloudRain,

    /// <summary>Icone <c>CloudSnowfall</c> do contrato com o RVM.UI, desenhado com o <c>cloud-snow</c> do Tabler.</summary>
    CloudSnowfall,

    /// <summary>Icone <c>CloudSnowfallMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>cloud-snow</c> do Tabler.</summary>
    CloudSnowfallMinimalistic,

    /// <summary>Icone <c>CloudStorage</c> do contrato com o RVM.UI, desenhado com o <c>cloud-data-connection</c> do Tabler.</summary>
    CloudStorage,

    /// <summary>Icone <c>CloudStorm</c> do contrato com o RVM.UI, desenhado com o <c>cloud-storm</c> do Tabler.</summary>
    CloudStorm,

    /// <summary>Icone <c>CloudSun</c> do contrato com o RVM.UI, desenhado com o <c>sun</c> do Tabler.</summary>
    CloudSun,

    /// <summary>Icone <c>CloudSunWeather</c> do contrato com o RVM.UI, desenhado com o <c>cloud</c> do Tabler.</summary>
    CloudSunWeather,

    /// <summary>Icone <c>CloudUpload</c> do contrato com o RVM.UI, desenhado com o <c>cloud-upload</c> do Tabler.</summary>
    CloudUpload,

    /// <summary>Icone <c>CloudWaterdrop</c> do contrato com o RVM.UI, desenhado com o <c>droplet</c> do Tabler.</summary>
    CloudWaterdrop,

    /// <summary>Icone <c>CloudWaterdrops</c> do contrato com o RVM.UI, desenhado com o <c>cloud-rain</c> do Tabler.</summary>
    CloudWaterdrops,

    /// <summary>Icone <c>Clouds</c> do contrato com o RVM.UI, desenhado com o <c>cloud</c> do Tabler.</summary>
    Clouds,

    /// <summary>Icone <c>CloudyMoon</c> do contrato com o RVM.UI, desenhado com o <c>moon-stars</c> do Tabler.</summary>
    CloudyMoon,

    /// <summary>Icone <c>CloundCross</c> do contrato com o RVM.UI, desenhado com o <c>cloud-x</c> do Tabler.</summary>
    CloundCross,

    /// <summary>Icone <c>Code</c> do contrato com o RVM.UI, desenhado com o <c>code</c> do Tabler.</summary>
    Code,

    /// <summary>Icone <c>Code2</c> do contrato com o RVM.UI, desenhado com o <c>code</c> do Tabler.</summary>
    Code2,

    /// <summary>Icone <c>CodeCircle</c> do contrato com o RVM.UI, desenhado com o <c>code-circle</c> do Tabler.</summary>
    CodeCircle,

    /// <summary>Icone <c>CodeFile</c> do contrato com o RVM.UI, desenhado com o <c>file-code</c> do Tabler.</summary>
    CodeFile,

    /// <summary>Icone <c>CodeScan</c> do contrato com o RVM.UI, desenhado com o <c>code</c> do Tabler.</summary>
    CodeScan,

    /// <summary>Icone <c>CodeSquare</c> do contrato com o RVM.UI, desenhado com o <c>code-circle</c> do Tabler.</summary>
    CodeSquare,

    /// <summary>Icone <c>ColourTuneing</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    ColourTuneing,

    /// <summary>Icone <c>Command</c> do contrato com o RVM.UI, desenhado com o <c>command</c> do Tabler.</summary>
    Command,

    /// <summary>Icone <c>Compass</c> do contrato com o RVM.UI, desenhado com o <c>compass</c> do Tabler.</summary>
    Compass,

    /// <summary>Icone <c>CompassBig</c> do contrato com o RVM.UI, desenhado com o <c>compass</c> do Tabler.</summary>
    CompassBig,

    /// <summary>Icone <c>CompassSquare</c> do contrato com o RVM.UI, desenhado com o <c>compass</c> do Tabler.</summary>
    CompassSquare,

    /// <summary>Icone <c>Condicioner</c> do contrato com o RVM.UI, desenhado com o <c>air-conditioning</c> do Tabler.</summary>
    Condicioner,

    /// <summary>Icone <c>Condicioner2</c> do contrato com o RVM.UI, desenhado com o <c>air-conditioning</c> do Tabler.</summary>
    Condicioner2,

    /// <summary>Icone <c>Confetti</c> do contrato com o RVM.UI, desenhado com o <c>confetti</c> do Tabler.</summary>
    Confetti,

    /// <summary>Icone <c>ConfettiMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>confetti</c> do Tabler.</summary>
    ConfettiMinimalistic,

    /// <summary>Icone <c>ConfoundedCircle</c> do contrato com o RVM.UI, desenhado com o <c>mood-confused</c> do Tabler.</summary>
    ConfoundedCircle,

    /// <summary>Icone <c>ConfoundedSquare</c> do contrato com o RVM.UI, desenhado com o <c>mood-confused</c> do Tabler.</summary>
    ConfoundedSquare,

    /// <summary>Icone <c>Copyright</c> do contrato com o RVM.UI, desenhado com o <c>copyright</c> do Tabler.</summary>
    Copyright,

    /// <summary>Icone <c>Corkscrew</c> do contrato com o RVM.UI, desenhado com o <c>bottle</c> do Tabler.</summary>
    Corkscrew,

    /// <summary>Icone <c>Cosmetic</c> do contrato com o RVM.UI, desenhado com o <c>brush</c> do Tabler.</summary>
    Cosmetic,

    /// <summary>Icone <c>CourseDown</c> do contrato com o RVM.UI, desenhado com o <c>trending-down</c> do Tabler.</summary>
    CourseDown,

    /// <summary>Icone <c>CourseUp</c> do contrato com o RVM.UI, desenhado com o <c>trending-up</c> do Tabler.</summary>
    CourseUp,

    /// <summary>Icone <c>Cpu</c> do contrato com o RVM.UI, desenhado com o <c>cpu</c> do Tabler.</summary>
    Cpu,

    /// <summary>Icone <c>CpuBolt</c> do contrato com o RVM.UI, desenhado com o <c>cpu</c> do Tabler.</summary>
    CpuBolt,

    /// <summary>Icone <c>CreativeCommons</c> do contrato com o RVM.UI, desenhado com o <c>creative-commons</c> do Tabler.</summary>
    CreativeCommons,

    /// <summary>Icone <c>Crop</c> do contrato com o RVM.UI, desenhado com o <c>crop</c> do Tabler.</summary>
    Crop,

    /// <summary>Icone <c>CropMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>crop</c> do Tabler.</summary>
    CropMinimalistic,

    /// <summary>Icone <c>Crown</c> do contrato com o RVM.UI, desenhado com o <c>crown</c> do Tabler.</summary>
    Crown,

    /// <summary>Icone <c>CrownLine</c> do contrato com o RVM.UI, desenhado com o <c>crown</c> do Tabler.</summary>
    CrownLine,

    /// <summary>Icone <c>CrownMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>crown</c> do Tabler.</summary>
    CrownMinimalistic,

    /// <summary>Icone <c>CrownStar</c> do contrato com o RVM.UI, desenhado com o <c>crown</c> do Tabler.</summary>
    CrownStar,

    /// <summary>Icone <c>Cup</c> do contrato com o RVM.UI, desenhado com o <c>cup</c> do Tabler.</summary>
    Cup,

    /// <summary>Icone <c>CupEssentionalUi</c> do contrato com o RVM.UI, desenhado com o <c>cup</c> do Tabler.</summary>
    CupEssentionalUi,

    /// <summary>Icone <c>CupFirst</c> do contrato com o RVM.UI, desenhado com o <c>trophy</c> do Tabler.</summary>
    CupFirst,

    /// <summary>Icone <c>CupHot</c> do contrato com o RVM.UI, desenhado com o <c>coffee</c> do Tabler.</summary>
    CupHot,

    /// <summary>Icone <c>CupMusic</c> do contrato com o RVM.UI, desenhado com o <c>trophy</c> do Tabler.</summary>
    CupMusic,

    /// <summary>Icone <c>CupPaper</c> do contrato com o RVM.UI, desenhado com o <c>cup</c> do Tabler.</summary>
    CupPaper,

    /// <summary>Icone <c>CupStar</c> do contrato com o RVM.UI, desenhado com o <c>trophy</c> do Tabler.</summary>
    CupStar,

    /// <summary>Icone <c>Cursor</c> do contrato com o RVM.UI, desenhado com o <c>pointer</c> do Tabler.</summary>
    Cursor,

    /// <summary>Icone <c>CursorSquare</c> do contrato com o RVM.UI, desenhado com o <c>pointer</c> do Tabler.</summary>
    CursorSquare,

    /// <summary>Icone <c>Danger</c> do contrato com o RVM.UI, desenhado com o <c>alert-triangle</c> do Tabler.</summary>
    Danger,

    /// <summary>Icone <c>DangerCircle</c> do contrato com o RVM.UI, desenhado com o <c>alert-circle</c> do Tabler.</summary>
    DangerCircle,

    /// <summary>Icone <c>DangerSquare</c> do contrato com o RVM.UI, desenhado com o <c>alert-square</c> do Tabler.</summary>
    DangerSquare,

    /// <summary>Icone <c>DangerTriangle</c> do contrato com o RVM.UI, desenhado com o <c>alert-triangle</c> do Tabler.</summary>
    DangerTriangle,

    /// <summary>Icone <c>Database</c> do contrato com o RVM.UI, desenhado com o <c>database</c> do Tabler.</summary>
    Database,

    /// <summary>Icone <c>Delivery</c> do contrato com o RVM.UI, desenhado com o <c>truck-delivery</c> do Tabler.</summary>
    Delivery,

    /// <summary>Icone <c>Devices</c> do contrato com o RVM.UI, desenhado com o <c>devices</c> do Tabler.</summary>
    Devices,

    /// <summary>Icone <c>DiagramDown</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    DiagramDown,

    /// <summary>Icone <c>DiagramUp</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    DiagramUp,

    /// <summary>Icone <c>Dialog</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    Dialog,

    /// <summary>Icone <c>Dialog2</c> do contrato com o RVM.UI, desenhado com o <c>message-2</c> do Tabler.</summary>
    Dialog2,

    /// <summary>Icone <c>Diploma</c> do contrato com o RVM.UI, desenhado com o <c>certificate</c> do Tabler.</summary>
    Diploma,

    /// <summary>Icone <c>DiplomaVerified</c> do contrato com o RVM.UI, desenhado com o <c>certificate</c> do Tabler.</summary>
    DiplomaVerified,

    /// <summary>Icone <c>Diskette</c> do contrato com o RVM.UI, desenhado com o <c>device-floppy</c> do Tabler.</summary>
    Diskette,

    /// <summary>Icone <c>Dislike</c> do contrato com o RVM.UI, desenhado com o <c>thumb-down</c> do Tabler.</summary>
    Dislike,

    /// <summary>Icone <c>Display</c> do contrato com o RVM.UI, desenhado com o <c>device-desktop</c> do Tabler.</summary>
    Display,

    /// <summary>Icone <c>Dna</c> do contrato com o RVM.UI, desenhado com o <c>dna</c> do Tabler.</summary>
    Dna,

    /// <summary>Icone <c>Document</c> do contrato com o RVM.UI, desenhado com o <c>file</c> do Tabler.</summary>
    Document,

    /// <summary>Icone <c>DocumentAdd</c> do contrato com o RVM.UI, desenhado com o <c>file-plus</c> do Tabler.</summary>
    DocumentAdd,

    /// <summary>Icone <c>DocumentMedicine</c> do contrato com o RVM.UI, desenhado com o <c>medical-cross</c> do Tabler.</summary>
    DocumentMedicine,

    /// <summary>Icone <c>DocumentNotes</c> do contrato com o RVM.UI, desenhado com o <c>notes</c> do Tabler.</summary>
    DocumentNotes,

    /// <summary>Icone <c>DocumentText</c> do contrato com o RVM.UI, desenhado com o <c>file-text</c> do Tabler.</summary>
    DocumentText,

    /// <summary>Icone <c>Documents</c> do contrato com o RVM.UI, desenhado com o <c>files</c> do Tabler.</summary>
    Documents,

    /// <summary>Icone <c>DocumentsMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>files</c> do Tabler.</summary>
    DocumentsMinimalistic,

    /// <summary>Icone <c>Dollar</c> do contrato com o RVM.UI, desenhado com o <c>currency-dollar</c> do Tabler.</summary>
    Dollar,

    /// <summary>Icone <c>DollarMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>currency-dollar</c> do Tabler.</summary>
    DollarMinimalistic,

    /// <summary>Icone <c>Donut</c> do contrato com o RVM.UI, desenhado com o <c>cookie</c> do Tabler.</summary>
    Donut,

    /// <summary>Icone <c>DonutBitton</c> do contrato com o RVM.UI, desenhado com o <c>cookie</c> do Tabler.</summary>
    DonutBitton,

    /// <summary>Icone <c>DoubleAltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>chevrons-down</c> do Tabler.</summary>
    DoubleAltArrowDown,

    /// <summary>Icone <c>DoubleAltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>chevrons-left</c> do Tabler.</summary>
    DoubleAltArrowLeft,

    /// <summary>Icone <c>DoubleAltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>chevrons-right</c> do Tabler.</summary>
    DoubleAltArrowRight,

    /// <summary>Icone <c>DoubleAltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>chevrons-up</c> do Tabler.</summary>
    DoubleAltArrowUp,

    /// <summary>Icone <c>DownloadMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>download</c> do Tabler.</summary>
    DownloadMinimalistic,

    /// <summary>Icone <c>DownloadSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    DownloadSquare,

    /// <summary>Icone <c>DownloadTwiceSquare</c> do contrato com o RVM.UI, desenhado com o <c>download</c> do Tabler.</summary>
    DownloadTwiceSquare,

    /// <summary>Icone <c>Dropper</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    Dropper,

    /// <summary>Icone <c>Dropper2</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    Dropper2,

    /// <summary>Icone <c>Dropper3</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    Dropper3,

    /// <summary>Icone <c>DropperMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    DropperMinimalistic,

    /// <summary>Icone <c>DropperMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    DropperMinimalistic2,

    /// <summary>Icone <c>Dumbbell</c> do contrato com o RVM.UI, desenhado com o <c>dumbbell</c> do Tabler.</summary>
    Dumbbell,

    /// <summary>Icone <c>DumbbellLarge</c> do contrato com o RVM.UI, desenhado com o <c>barbell</c> do Tabler.</summary>
    DumbbellLarge,

    /// <summary>Icone <c>DumbbellLargeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>barbell</c> do Tabler.</summary>
    DumbbellLargeMinimalistic,

    /// <summary>Icone <c>DumbbellSmall</c> do contrato com o RVM.UI, desenhado com o <c>barbell</c> do Tabler.</summary>
    DumbbellSmall,

    /// <summary>Icone <c>Dumbbells</c> do contrato com o RVM.UI, desenhado com o <c>barbell</c> do Tabler.</summary>
    Dumbbells,

    /// <summary>Icone <c>Dumbbells2</c> do contrato com o RVM.UI, desenhado com o <c>barbell</c> do Tabler.</summary>
    Dumbbells2,

    /// <summary>Icone <c>Earth</c> do contrato com o RVM.UI, desenhado com o <c>world</c> do Tabler.</summary>
    Earth,

    /// <summary>Icone <c>ElectricRefueling</c> do contrato com o RVM.UI, desenhado com o <c>charging-pile</c> do Tabler.</summary>
    ElectricRefueling,

    /// <summary>Icone <c>EmojiFunnyCircle</c> do contrato com o RVM.UI, desenhado com o <c>mood-happy</c> do Tabler.</summary>
    EmojiFunnyCircle,

    /// <summary>Icone <c>EmojiFunnySquare</c> do contrato com o RVM.UI, desenhado com o <c>mood-happy</c> do Tabler.</summary>
    EmojiFunnySquare,

    /// <summary>Icone <c>EndCall</c> do contrato com o RVM.UI, desenhado com o <c>phone-off</c> do Tabler.</summary>
    EndCall,

    /// <summary>Icone <c>EndCallRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-off</c> do Tabler.</summary>
    EndCallRounded,

    /// <summary>Icone <c>Eraser</c> do contrato com o RVM.UI, desenhado com o <c>eraser</c> do Tabler.</summary>
    Eraser,

    /// <summary>Icone <c>EraserCircle</c> do contrato com o RVM.UI, desenhado com o <c>eraser</c> do Tabler.</summary>
    EraserCircle,

    /// <summary>Icone <c>EraserSquare</c> do contrato com o RVM.UI, desenhado com o <c>eraser</c> do Tabler.</summary>
    EraserSquare,

    /// <summary>Icone <c>Euro</c> do contrato com o RVM.UI, desenhado com o <c>currency-euro</c> do Tabler.</summary>
    Euro,

    /// <summary>Icone <c>Exit</c> do contrato com o RVM.UI, desenhado com o <c>logout</c> do Tabler.</summary>
    Exit,

    /// <summary>Icone <c>Explicit</c> do contrato com o RVM.UI, desenhado com o <c>explicit</c> do Tabler.</summary>
    Explicit,

    /// <summary>Icone <c>Export</c> do contrato com o RVM.UI, desenhado com o <c>file-export</c> do Tabler.</summary>
    Export,

    /// <summary>Icone <c>ExpressionlessCircle</c> do contrato com o RVM.UI, desenhado com o <c>mood-neutral</c> do Tabler.</summary>
    ExpressionlessCircle,

    /// <summary>Icone <c>ExpressionlessSquare</c> do contrato com o RVM.UI, desenhado com o <c>mood-neutral</c> do Tabler.</summary>
    ExpressionlessSquare,

    /// <summary>Icone <c>EyeClosed</c> do contrato com o RVM.UI, desenhado com o <c>eye-closed</c> do Tabler.</summary>
    EyeClosed,

    /// <summary>Icone <c>EyeScan</c> do contrato com o RVM.UI, desenhado com o <c>scan</c> do Tabler.</summary>
    EyeScan,

    /// <summary>Icone <c>FaceScanCircle</c> do contrato com o RVM.UI, desenhado com o <c>face-id</c> do Tabler.</summary>
    FaceScanCircle,

    /// <summary>Icone <c>FaceScanSquare</c> do contrato com o RVM.UI, desenhado com o <c>face-id</c> do Tabler.</summary>
    FaceScanSquare,

    /// <summary>Icone <c>FacemaskCircle</c> do contrato com o RVM.UI, desenhado com o <c>virus</c> do Tabler.</summary>
    FacemaskCircle,

    /// <summary>Icone <c>FacemaskSquare</c> do contrato com o RVM.UI, desenhado com o <c>virus</c> do Tabler.</summary>
    FacemaskSquare,

    /// <summary>Icone <c>Feed</c> do contrato com o RVM.UI, desenhado com o <c>rss</c> do Tabler.</summary>
    Feed,

    /// <summary>Icone <c>FerrisWheel</c> do contrato com o RVM.UI, desenhado com o <c>carousel-horizontal</c> do Tabler.</summary>
    FerrisWheel,

    /// <summary>Icone <c>Figma</c> do contrato com o RVM.UI, desenhado com o <c>brand-figma</c> do Tabler.</summary>
    Figma,

    /// <summary>Icone <c>FigmaFile</c> do contrato com o RVM.UI, desenhado com o <c>brand-figma</c> do Tabler.</summary>
    FigmaFile,

    /// <summary>Icone <c>FileCheck</c> do contrato com o RVM.UI, desenhado com o <c>file-check</c> do Tabler.</summary>
    FileCheck,

    /// <summary>Icone <c>FileCorrupted</c> do contrato com o RVM.UI, desenhado com o <c>file-alert</c> do Tabler.</summary>
    FileCorrupted,

    /// <summary>Icone <c>FileDownload</c> do contrato com o RVM.UI, desenhado com o <c>file-download</c> do Tabler.</summary>
    FileDownload,

    /// <summary>Icone <c>FileFavourite</c> do contrato com o RVM.UI, desenhado com o <c>file-like</c> do Tabler.</summary>
    FileFavourite,

    /// <summary>Icone <c>FileLeft</c> do contrato com o RVM.UI, desenhado com o <c>file-arrow-left</c> do Tabler.</summary>
    FileLeft,

    /// <summary>Icone <c>FileRemove</c> do contrato com o RVM.UI, desenhado com o <c>file-x</c> do Tabler.</summary>
    FileRemove,

    /// <summary>Icone <c>FileRight</c> do contrato com o RVM.UI, desenhado com o <c>file-arrow-right</c> do Tabler.</summary>
    FileRight,

    /// <summary>Icone <c>FileSend</c> do contrato com o RVM.UI, desenhado com o <c>file-upload</c> do Tabler.</summary>
    FileSend,

    /// <summary>Icone <c>FileSmile</c> do contrato com o RVM.UI, desenhado com o <c>file-smile</c> do Tabler.</summary>
    FileSmile,

    /// <summary>Icone <c>FileText</c> do contrato com o RVM.UI, desenhado com o <c>file-text</c> do Tabler.</summary>
    FileText,

    /// <summary>Icone <c>Filters</c> do contrato com o RVM.UI, desenhado com o <c>filters</c> do Tabler.</summary>
    Filters,

    /// <summary>Icone <c>Fire</c> do contrato com o RVM.UI, desenhado com o <c>flame</c> do Tabler.</summary>
    Fire,

    /// <summary>Icone <c>FireMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>flame</c> do Tabler.</summary>
    FireMinimalistic,

    /// <summary>Icone <c>FireSquare</c> do contrato com o RVM.UI, desenhado com o <c>flame</c> do Tabler.</summary>
    FireSquare,

    /// <summary>Icone <c>Flag</c> do contrato com o RVM.UI, desenhado com o <c>flag</c> do Tabler.</summary>
    Flag,

    /// <summary>Icone <c>Flag2</c> do contrato com o RVM.UI, desenhado com o <c>flag-2</c> do Tabler.</summary>
    Flag2,

    /// <summary>Icone <c>Flame</c> do contrato com o RVM.UI, desenhado com o <c>flame</c> do Tabler.</summary>
    Flame,

    /// <summary>Icone <c>Flashlight</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    Flashlight,

    /// <summary>Icone <c>FlashlightOn</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    FlashlightOn,

    /// <summary>Icone <c>FlipHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>flip-horizontal</c> do Tabler.</summary>
    FlipHorizontal,

    /// <summary>Icone <c>FlipVertical</c> do contrato com o RVM.UI, desenhado com o <c>flip-vertical</c> do Tabler.</summary>
    FlipVertical,

    /// <summary>Icone <c>FloorLamp</c> do contrato com o RVM.UI, desenhado com o <c>lamp-2</c> do Tabler.</summary>
    FloorLamp,

    /// <summary>Icone <c>FloorLampMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>lamp</c> do Tabler.</summary>
    FloorLampMinimalistic,

    /// <summary>Icone <c>Fog</c> do contrato com o RVM.UI, desenhado com o <c>mist</c> do Tabler.</summary>
    Fog,

    /// <summary>Icone <c>Folder</c> do contrato com o RVM.UI, desenhado com o <c>folder</c> do Tabler.</summary>
    Folder,

    /// <summary>Icone <c>Folder2</c> do contrato com o RVM.UI, desenhado com o <c>folder</c> do Tabler.</summary>
    Folder2,

    /// <summary>Icone <c>FolderAdd</c> do contrato com o RVM.UI, desenhado com o <c>folder-plus</c> do Tabler.</summary>
    FolderAdd,

    /// <summary>Icone <c>FolderCheck</c> do contrato com o RVM.UI, desenhado com o <c>folder-check</c> do Tabler.</summary>
    FolderCheck,

    /// <summary>Icone <c>FolderCloud</c> do contrato com o RVM.UI, desenhado com o <c>cloud</c> do Tabler.</summary>
    FolderCloud,

    /// <summary>Icone <c>FolderError</c> do contrato com o RVM.UI, desenhado com o <c>folder-x</c> do Tabler.</summary>
    FolderError,

    /// <summary>Icone <c>FolderFavouriteBookmark</c> do contrato com o RVM.UI, desenhado com o <c>folder-heart</c> do Tabler.</summary>
    FolderFavouriteBookmark,

    /// <summary>Icone <c>FolderFavouriteStar</c> do contrato com o RVM.UI, desenhado com o <c>folder-star</c> do Tabler.</summary>
    FolderFavouriteStar,

    /// <summary>Icone <c>FolderMove</c> do contrato com o RVM.UI, desenhado com o <c>folder-share</c> do Tabler.</summary>
    FolderMove,

    /// <summary>Icone <c>FolderOpen</c> do contrato com o RVM.UI, desenhado com o <c>folder-open</c> do Tabler.</summary>
    FolderOpen,

    /// <summary>Icone <c>FolderPathConnect</c> do contrato com o RVM.UI, desenhado com o <c>folder-symlink</c> do Tabler.</summary>
    FolderPathConnect,

    /// <summary>Icone <c>FolderSecurity</c> do contrato com o RVM.UI, desenhado com o <c>folder-lock</c> do Tabler.</summary>
    FolderSecurity,

    /// <summary>Icone <c>FolderWithFiles</c> do contrato com o RVM.UI, desenhado com o <c>folders</c> do Tabler.</summary>
    FolderWithFiles,

    /// <summary>Icone <c>Football</c> do contrato com o RVM.UI, desenhado com o <c>ball-football</c> do Tabler.</summary>
    Football,

    /// <summary>Icone <c>Forbidden</c> do contrato com o RVM.UI, desenhado com o <c>ban</c> do Tabler.</summary>
    Forbidden,

    /// <summary>Icone <c>ForbiddenCircle</c> do contrato com o RVM.UI, desenhado com o <c>ban</c> do Tabler.</summary>
    ForbiddenCircle,

    /// <summary>Icone <c>Forward</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    Forward,

    /// <summary>Icone <c>Forward2</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward</c> do Tabler.</summary>
    Forward2,

    /// <summary>Icone <c>ForwardArrowsAction</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward</c> do Tabler.</summary>
    ForwardArrowsAction,

    /// <summary>Icone <c>Fridge</c> do contrato com o RVM.UI, desenhado com o <c>fridge</c> do Tabler.</summary>
    Fridge,

    /// <summary>Icone <c>Fuel</c> do contrato com o RVM.UI, desenhado com o <c>gas-station</c> do Tabler.</summary>
    Fuel,

    /// <summary>Icone <c>FullScreen</c> do contrato com o RVM.UI, desenhado com o <c>maximize</c> do Tabler.</summary>
    FullScreen,

    /// <summary>Icone <c>FullScreenCircle</c> do contrato com o RVM.UI, desenhado com o <c>maximize</c> do Tabler.</summary>
    FullScreenCircle,

    /// <summary>Icone <c>FullScreenSquare</c> do contrato com o RVM.UI, desenhado com o <c>maximize</c> do Tabler.</summary>
    FullScreenSquare,

    /// <summary>Icone <c>Gallery</c> do contrato com o RVM.UI, desenhado com o <c>photo</c> do Tabler.</summary>
    Gallery,

    /// <summary>Icone <c>GalleryAdd</c> do contrato com o RVM.UI, desenhado com o <c>photo-plus</c> do Tabler.</summary>
    GalleryAdd,

    /// <summary>Icone <c>GalleryCheck</c> do contrato com o RVM.UI, desenhado com o <c>photo-check</c> do Tabler.</summary>
    GalleryCheck,

    /// <summary>Icone <c>GalleryCircle</c> do contrato com o RVM.UI, desenhado com o <c>photo</c> do Tabler.</summary>
    GalleryCircle,

    /// <summary>Icone <c>GalleryDownload</c> do contrato com o RVM.UI, desenhado com o <c>photo-down</c> do Tabler.</summary>
    GalleryDownload,

    /// <summary>Icone <c>GalleryEdit</c> do contrato com o RVM.UI, desenhado com o <c>photo-edit</c> do Tabler.</summary>
    GalleryEdit,

    /// <summary>Icone <c>GalleryFavourite</c> do contrato com o RVM.UI, desenhado com o <c>photo-heart</c> do Tabler.</summary>
    GalleryFavourite,

    /// <summary>Icone <c>GalleryMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>photo</c> do Tabler.</summary>
    GalleryMinimalistic,

    /// <summary>Icone <c>GalleryRemove</c> do contrato com o RVM.UI, desenhado com o <c>photo-x</c> do Tabler.</summary>
    GalleryRemove,

    /// <summary>Icone <c>GalleryRound</c> do contrato com o RVM.UI, desenhado com o <c>photo</c> do Tabler.</summary>
    GalleryRound,

    /// <summary>Icone <c>GallerySend</c> do contrato com o RVM.UI, desenhado com o <c>photo-up</c> do Tabler.</summary>
    GallerySend,

    /// <summary>Icone <c>GalleryWide</c> do contrato com o RVM.UI, desenhado com o <c>photo</c> do Tabler.</summary>
    GalleryWide,

    /// <summary>Icone <c>Gameboy</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad</c> do Tabler.</summary>
    Gameboy,

    /// <summary>Icone <c>Gamepad</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad-2</c> do Tabler.</summary>
    Gamepad,

    /// <summary>Icone <c>GamepadCharge</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad</c> do Tabler.</summary>
    GamepadCharge,

    /// <summary>Icone <c>GamepadMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad</c> do Tabler.</summary>
    GamepadMinimalistic,

    /// <summary>Icone <c>GamepadNoCharge</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad</c> do Tabler.</summary>
    GamepadNoCharge,

    /// <summary>Icone <c>GamepadOld</c> do contrato com o RVM.UI, desenhado com o <c>device-gamepad</c> do Tabler.</summary>
    GamepadOld,

    /// <summary>Icone <c>Garage</c> do contrato com o RVM.UI, desenhado com o <c>building-warehouse</c> do Tabler.</summary>
    Garage,

    /// <summary>Icone <c>GasStation</c> do contrato com o RVM.UI, desenhado com o <c>gas-station</c> do Tabler.</summary>
    GasStation,

    /// <summary>Icone <c>Ghost</c> do contrato com o RVM.UI, desenhado com o <c>ghost</c> do Tabler.</summary>
    Ghost,

    /// <summary>Icone <c>GhostSmile</c> do contrato com o RVM.UI, desenhado com o <c>ghost</c> do Tabler.</summary>
    GhostSmile,

    /// <summary>Icone <c>Gift</c> do contrato com o RVM.UI, desenhado com o <c>gift</c> do Tabler.</summary>
    Gift,

    /// <summary>Icone <c>Glasses</c> do contrato com o RVM.UI, desenhado com o <c>eyeglass</c> do Tabler.</summary>
    Glasses,

    /// <summary>Icone <c>Global</c> do contrato com o RVM.UI, desenhado com o <c>world</c> do Tabler.</summary>
    Global,

    /// <summary>Icone <c>Globus</c> do contrato com o RVM.UI, desenhado com o <c>world</c> do Tabler.</summary>
    Globus,

    /// <summary>Icone <c>Golf</c> do contrato com o RVM.UI, desenhado com o <c>golf</c> do Tabler.</summary>
    Golf,

    /// <summary>Icone <c>Gps</c> do contrato com o RVM.UI, desenhado com o <c>gps</c> do Tabler.</summary>
    Gps,

    /// <summary>Icone <c>Graph</c> do contrato com o RVM.UI, desenhado com o <c>graph</c> do Tabler.</summary>
    Graph,

    /// <summary>Icone <c>GraphDown</c> do contrato com o RVM.UI, desenhado com o <c>trending-down</c> do Tabler.</summary>
    GraphDown,

    /// <summary>Icone <c>GraphDownNew</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    GraphDownNew,

    /// <summary>Icone <c>GraphNew</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    GraphNew,

    /// <summary>Icone <c>GraphNewUp</c> do contrato com o RVM.UI, desenhado com o <c>chart-line</c> do Tabler.</summary>
    GraphNewUp,

    /// <summary>Icone <c>GraphUp</c> do contrato com o RVM.UI, desenhado com o <c>trending-up</c> do Tabler.</summary>
    GraphUp,

    /// <summary>Icone <c>HamburgerMenu</c> do contrato com o RVM.UI, desenhado com o <c>menu-2</c> do Tabler.</summary>
    HamburgerMenu,

    /// <summary>Icone <c>HandHeart</c> do contrato com o RVM.UI, desenhado com o <c>heart-handshake</c> do Tabler.</summary>
    HandHeart,

    /// <summary>Icone <c>HandMoney</c> do contrato com o RVM.UI, desenhado com o <c>businessplan</c> do Tabler.</summary>
    HandMoney,

    /// <summary>Icone <c>HandPills</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    HandPills,

    /// <summary>Icone <c>HandShake</c> do contrato com o RVM.UI, desenhado com o <c>heart-handshake</c> do Tabler.</summary>
    HandShake,

    /// <summary>Icone <c>HandStars</c> do contrato com o RVM.UI, desenhado com o <c>hand-love-you</c> do Tabler.</summary>
    HandStars,

    /// <summary>Icone <c>Hanger</c> do contrato com o RVM.UI, desenhado com o <c>hanger</c> do Tabler.</summary>
    Hanger,

    /// <summary>Icone <c>Hanger2</c> do contrato com o RVM.UI, desenhado com o <c>hanger-2</c> do Tabler.</summary>
    Hanger2,

    /// <summary>Icone <c>Hashtag</c> do contrato com o RVM.UI, desenhado com o <c>hash</c> do Tabler.</summary>
    Hashtag,

    /// <summary>Icone <c>HashtagChat</c> do contrato com o RVM.UI, desenhado com o <c>message</c> do Tabler.</summary>
    HashtagChat,

    /// <summary>Icone <c>HashtagCircle</c> do contrato com o RVM.UI, desenhado com o <c>hash</c> do Tabler.</summary>
    HashtagCircle,

    /// <summary>Icone <c>HashtagSquare</c> do contrato com o RVM.UI, desenhado com o <c>hash</c> do Tabler.</summary>
    HashtagSquare,

    /// <summary>Icone <c>HeadphonesRound</c> do contrato com o RVM.UI, desenhado com o <c>headphones</c> do Tabler.</summary>
    HeadphonesRound,

    /// <summary>Icone <c>HeadphonesRoundSound</c> do contrato com o RVM.UI, desenhado com o <c>headphones</c> do Tabler.</summary>
    HeadphonesRoundSound,

    /// <summary>Icone <c>HeadphonesSquare</c> do contrato com o RVM.UI, desenhado com o <c>headphones</c> do Tabler.</summary>
    HeadphonesSquare,

    /// <summary>Icone <c>HeadphonesSquareSound</c> do contrato com o RVM.UI, desenhado com o <c>headphones</c> do Tabler.</summary>
    HeadphonesSquareSound,

    /// <summary>Icone <c>Health</c> do contrato com o RVM.UI, desenhado com o <c>heart-plus</c> do Tabler.</summary>
    Health,

    /// <summary>Icone <c>HeartAngle</c> do contrato com o RVM.UI, desenhado com o <c>heart</c> do Tabler.</summary>
    HeartAngle,

    /// <summary>Icone <c>HeartBroken</c> do contrato com o RVM.UI, desenhado com o <c>heart-broken</c> do Tabler.</summary>
    HeartBroken,

    /// <summary>Icone <c>HeartLock</c> do contrato com o RVM.UI, desenhado com o <c>lock-heart</c> do Tabler.</summary>
    HeartLock,

    /// <summary>Icone <c>HeartPulse</c> do contrato com o RVM.UI, desenhado com o <c>heart-rate-monitor</c> do Tabler.</summary>
    HeartPulse,

    /// <summary>Icone <c>HeartPulse2</c> do contrato com o RVM.UI, desenhado com o <c>heartbeat</c> do Tabler.</summary>
    HeartPulse2,

    /// <summary>Icone <c>HeartShine</c> do contrato com o RVM.UI, desenhado com o <c>heart</c> do Tabler.</summary>
    HeartShine,

    /// <summary>Icone <c>HeartUnlock</c> do contrato com o RVM.UI, desenhado com o <c>heart</c> do Tabler.</summary>
    HeartUnlock,

    /// <summary>Icone <c>Hearts</c> do contrato com o RVM.UI, desenhado com o <c>hearts</c> do Tabler.</summary>
    Hearts,

    /// <summary>Icone <c>Help</c> do contrato com o RVM.UI, desenhado com o <c>help</c> do Tabler.</summary>
    Help,

    /// <summary>Icone <c>HighDefinition</c> do contrato com o RVM.UI, desenhado com o <c>badge-hd</c> do Tabler.</summary>
    HighDefinition,

    /// <summary>Icone <c>HighQuality</c> do contrato com o RVM.UI, desenhado com o <c>badge-hd</c> do Tabler.</summary>
    HighQuality,

    /// <summary>Icone <c>Hiking</c> do contrato com o RVM.UI, desenhado com o <c>backpack</c> do Tabler.</summary>
    Hiking,

    /// <summary>Icone <c>HikingMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>backpack</c> do Tabler.</summary>
    HikingMinimalistic,

    /// <summary>Icone <c>HikingRound</c> do contrato com o RVM.UI, desenhado com o <c>backpack</c> do Tabler.</summary>
    HikingRound,

    /// <summary>Icone <c>History</c> do contrato com o RVM.UI, desenhado com o <c>history</c> do Tabler.</summary>
    History,

    /// <summary>Icone <c>History2</c> do contrato com o RVM.UI, desenhado com o <c>history</c> do Tabler.</summary>
    History2,

    /// <summary>Icone <c>History3</c> do contrato com o RVM.UI, desenhado com o <c>history</c> do Tabler.</summary>
    History3,

    /// <summary>Icone <c>Hive</c> do contrato com o RVM.UI, desenhado com o <c>hexagon</c> do Tabler.</summary>
    Hive,

    /// <summary>Icone <c>Home2</c> do contrato com o RVM.UI, desenhado com o <c>home-2</c> do Tabler.</summary>
    Home2,

    /// <summary>Icone <c>HomeAdd</c> do contrato com o RVM.UI, desenhado com o <c>home-plus</c> do Tabler.</summary>
    HomeAdd,

    /// <summary>Icone <c>HomeAddAngle</c> do contrato com o RVM.UI, desenhado com o <c>home-plus</c> do Tabler.</summary>
    HomeAddAngle,

    /// <summary>Icone <c>HomeAngle</c> do contrato com o RVM.UI, desenhado com o <c>home</c> do Tabler.</summary>
    HomeAngle,

    /// <summary>Icone <c>HomeAngle2</c> do contrato com o RVM.UI, desenhado com o <c>home-2</c> do Tabler.</summary>
    HomeAngle2,

    /// <summary>Icone <c>HomeBuildingInfrastructure</c> do contrato com o RVM.UI, desenhado com o <c>building-community</c> do Tabler.</summary>
    HomeBuildingInfrastructure,

    /// <summary>Icone <c>HomeSmile</c> do contrato com o RVM.UI, desenhado com o <c>home-heart</c> do Tabler.</summary>
    HomeSmile,

    /// <summary>Icone <c>HomeSmileAngle</c> do contrato com o RVM.UI, desenhado com o <c>home-heart</c> do Tabler.</summary>
    HomeSmileAngle,

    /// <summary>Icone <c>HomeWifi</c> do contrato com o RVM.UI, desenhado com o <c>home-signal</c> do Tabler.</summary>
    HomeWifi,

    /// <summary>Icone <c>HomeWifiAngle</c> do contrato com o RVM.UI, desenhado com o <c>home-signal</c> do Tabler.</summary>
    HomeWifiAngle,

    /// <summary>Icone <c>Hospital</c> do contrato com o RVM.UI, desenhado com o <c>hospital</c> do Tabler.</summary>
    Hospital,

    /// <summary>Icone <c>Hourglass</c> do contrato com o RVM.UI, desenhado com o <c>hourglass</c> do Tabler.</summary>
    Hourglass,

    /// <summary>Icone <c>HourglassLine</c> do contrato com o RVM.UI, desenhado com o <c>hourglass</c> do Tabler.</summary>
    HourglassLine,

    /// <summary>Icone <c>Import</c> do contrato com o RVM.UI, desenhado com o <c>file-import</c> do Tabler.</summary>
    Import,

    /// <summary>Icone <c>Inbox</c> do contrato com o RVM.UI, desenhado com o <c>inbox</c> do Tabler.</summary>
    Inbox,

    /// <summary>Icone <c>InboxArchive</c> do contrato com o RVM.UI, desenhado com o <c>archive</c> do Tabler.</summary>
    InboxArchive,

    /// <summary>Icone <c>InboxIn</c> do contrato com o RVM.UI, desenhado com o <c>inbox</c> do Tabler.</summary>
    InboxIn,

    /// <summary>Icone <c>InboxLine</c> do contrato com o RVM.UI, desenhado com o <c>inbox</c> do Tabler.</summary>
    InboxLine,

    /// <summary>Icone <c>InboxOut</c> do contrato com o RVM.UI, desenhado com o <c>mail-forward</c> do Tabler.</summary>
    InboxOut,

    /// <summary>Icone <c>InboxUnread</c> do contrato com o RVM.UI, desenhado com o <c>mail-opened</c> do Tabler.</summary>
    InboxUnread,

    /// <summary>Icone <c>Incognito</c> do contrato com o RVM.UI, desenhado com o <c>spy</c> do Tabler.</summary>
    Incognito,

    /// <summary>Icone <c>IncomingCall</c> do contrato com o RVM.UI, desenhado com o <c>phone-incoming</c> do Tabler.</summary>
    IncomingCall,

    /// <summary>Icone <c>IncomingCallRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-incoming</c> do Tabler.</summary>
    IncomingCallRounded,

    /// <summary>Icone <c>Infinity</c> do contrato com o RVM.UI, desenhado com o <c>infinity</c> do Tabler.</summary>
    Infinity,

    /// <summary>Icone <c>InfoSquare</c> do contrato com o RVM.UI, desenhado com o <c>info-square</c> do Tabler.</summary>
    InfoSquare,

    /// <summary>Icone <c>Iphone</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile</c> do Tabler.</summary>
    Iphone,

    /// <summary>Icone <c>JarOfPills</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    JarOfPills,

    /// <summary>Icone <c>JarOfPills2</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    JarOfPills2,

    /// <summary>Icone <c>Key</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    Key,

    /// <summary>Icone <c>KeyMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeyMinimalistic,

    /// <summary>Icone <c>KeyMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeyMinimalistic2,

    /// <summary>Icone <c>KeyMinimalisticSquare</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeyMinimalisticSquare,

    /// <summary>Icone <c>KeyMinimalisticSquare2</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeyMinimalisticSquare2,

    /// <summary>Icone <c>KeyMinimalisticSquare3</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeyMinimalisticSquare3,

    /// <summary>Icone <c>KeySquare</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeySquare,

    /// <summary>Icone <c>KeySquare2</c> do contrato com o RVM.UI, desenhado com o <c>key</c> do Tabler.</summary>
    KeySquare2,

    /// <summary>Icone <c>Keyboard</c> do contrato com o RVM.UI, desenhado com o <c>keyboard</c> do Tabler.</summary>
    Keyboard,

    /// <summary>Icone <c>KickScooter</c> do contrato com o RVM.UI, desenhado com o <c>scooter</c> do Tabler.</summary>
    KickScooter,

    /// <summary>Icone <c>Ladle</c> do contrato com o RVM.UI, desenhado com o <c>ladle</c> do Tabler.</summary>
    Ladle,

    /// <summary>Icone <c>Lamp</c> do contrato com o RVM.UI, desenhado com o <c>lamp</c> do Tabler.</summary>
    Lamp,

    /// <summary>Icone <c>Layers</c> do contrato com o RVM.UI, desenhado com o <c>stack-2</c> do Tabler.</summary>
    Layers,

    /// <summary>Icone <c>LayersMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>stack</c> do Tabler.</summary>
    LayersMinimalistic,

    /// <summary>Icone <c>Leaf</c> do contrato com o RVM.UI, desenhado com o <c>leaf</c> do Tabler.</summary>
    Leaf,

    /// <summary>Icone <c>Letter</c> do contrato com o RVM.UI, desenhado com o <c>mail</c> do Tabler.</summary>
    Letter,

    /// <summary>Icone <c>LetterOpened</c> do contrato com o RVM.UI, desenhado com o <c>mail-opened</c> do Tabler.</summary>
    LetterOpened,

    /// <summary>Icone <c>LetterUnread</c> do contrato com o RVM.UI, desenhado com o <c>mail</c> do Tabler.</summary>
    LetterUnread,

    /// <summary>Icone <c>Library</c> do contrato com o RVM.UI, desenhado com o <c>library</c> do Tabler.</summary>
    Library,

    /// <summary>Icone <c>Lightbulb</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    Lightbulb,

    /// <summary>Icone <c>LightbulbBolt</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    LightbulbBolt,

    /// <summary>Icone <c>LightbulbMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>bulb</c> do Tabler.</summary>
    LightbulbMinimalistic,

    /// <summary>Icone <c>Lightning</c> do contrato com o RVM.UI, desenhado com o <c>bolt</c> do Tabler.</summary>
    Lightning,

    /// <summary>Icone <c>Like</c> do contrato com o RVM.UI, desenhado com o <c>thumb-up</c> do Tabler.</summary>
    Like,

    /// <summary>Icone <c>Link</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    Link,

    /// <summary>Icone <c>LinkBroken</c> do contrato com o RVM.UI, desenhado com o <c>unlink</c> do Tabler.</summary>
    LinkBroken,

    /// <summary>Icone <c>LinkBrokenMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>unlink</c> do Tabler.</summary>
    LinkBrokenMinimalistic,

    /// <summary>Icone <c>LinkCircle</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    LinkCircle,

    /// <summary>Icone <c>LinkMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    LinkMinimalistic,

    /// <summary>Icone <c>LinkMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    LinkMinimalistic2,

    /// <summary>Icone <c>LinkRound</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    LinkRound,

    /// <summary>Icone <c>LinkRoundAngle</c> do contrato com o RVM.UI, desenhado com o <c>link</c> do Tabler.</summary>
    LinkRoundAngle,

    /// <summary>Icone <c>LinkSquare</c> do contrato com o RVM.UI, desenhado com o <c>external-link</c> do Tabler.</summary>
    LinkSquare,

    /// <summary>Icone <c>List</c> do contrato com o RVM.UI, desenhado com o <c>list</c> do Tabler.</summary>
    List,

    /// <summary>Icone <c>ListArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>sort-descending</c> do Tabler.</summary>
    ListArrowDown,

    /// <summary>Icone <c>ListArrowDownMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>sort-descending</c> do Tabler.</summary>
    ListArrowDownMinimalistic,

    /// <summary>Icone <c>ListArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    ListArrowUp,

    /// <summary>Icone <c>ListArrowUpMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    ListArrowUpMinimalistic,

    /// <summary>Icone <c>ListCheck</c> do contrato com o RVM.UI, desenhado com o <c>list-check</c> do Tabler.</summary>
    ListCheck,

    /// <summary>Icone <c>ListCheckMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>list-check</c> do Tabler.</summary>
    ListCheckMinimalistic,

    /// <summary>Icone <c>ListCross</c> do contrato com o RVM.UI, desenhado com o <c>list</c> do Tabler.</summary>
    ListCross,

    /// <summary>Icone <c>ListCrossMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>list</c> do Tabler.</summary>
    ListCrossMinimalistic,

    /// <summary>Icone <c>ListDown</c> do contrato com o RVM.UI, desenhado com o <c>sort-descending</c> do Tabler.</summary>
    ListDown,

    /// <summary>Icone <c>ListHeart</c> do contrato com o RVM.UI, desenhado com o <c>heart</c> do Tabler.</summary>
    ListHeart,

    /// <summary>Icone <c>ListHeartMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>heart</c> do Tabler.</summary>
    ListHeartMinimalistic,

    /// <summary>Icone <c>ListUp</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    ListUp,

    /// <summary>Icone <c>ListUpMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    ListUpMinimalistic,

    /// <summary>Icone <c>ListUpMinimalisticList</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    ListUpMinimalisticList,

    /// <summary>Icone <c>LockKeyhole</c> do contrato com o RVM.UI, desenhado com o <c>lock</c> do Tabler.</summary>
    LockKeyhole,

    /// <summary>Icone <c>LockKeyholeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>lock</c> do Tabler.</summary>
    LockKeyholeMinimalistic,

    /// <summary>Icone <c>LockKeyholeMinimalisticUnlocked</c> do contrato com o RVM.UI, desenhado com o <c>lock-open</c> do Tabler.</summary>
    LockKeyholeMinimalisticUnlocked,

    /// <summary>Icone <c>LockKeyholeUnlocked</c> do contrato com o RVM.UI, desenhado com o <c>lock-open</c> do Tabler.</summary>
    LockKeyholeUnlocked,

    /// <summary>Icone <c>LockPassword</c> do contrato com o RVM.UI, desenhado com o <c>lock-password</c> do Tabler.</summary>
    LockPassword,

    /// <summary>Icone <c>LockPasswordUnlocked</c> do contrato com o RVM.UI, desenhado com o <c>lock-open</c> do Tabler.</summary>
    LockPasswordUnlocked,

    /// <summary>Icone <c>LockUnlocked</c> do contrato com o RVM.UI, desenhado com o <c>lock-open</c> do Tabler.</summary>
    LockUnlocked,

    /// <summary>Icone <c>Login</c> do contrato com o RVM.UI, desenhado com o <c>login</c> do Tabler.</summary>
    Login,

    /// <summary>Icone <c>Login2</c> do contrato com o RVM.UI, desenhado com o <c>login-2</c> do Tabler.</summary>
    Login2,

    /// <summary>Icone <c>Login3</c> do contrato com o RVM.UI, desenhado com o <c>login</c> do Tabler.</summary>
    Login3,

    /// <summary>Icone <c>Logout2</c> do contrato com o RVM.UI, desenhado com o <c>logout-2</c> do Tabler.</summary>
    Logout2,

    /// <summary>Icone <c>Logout3</c> do contrato com o RVM.UI, desenhado com o <c>logout</c> do Tabler.</summary>
    Logout3,

    /// <summary>Icone <c>MagicStick</c> do contrato com o RVM.UI, desenhado com o <c>wand</c> do Tabler.</summary>
    MagicStick,

    /// <summary>Icone <c>MagicStick2</c> do contrato com o RVM.UI, desenhado com o <c>wand</c> do Tabler.</summary>
    MagicStick2,

    /// <summary>Icone <c>MagicStick3</c> do contrato com o RVM.UI, desenhado com o <c>wand</c> do Tabler.</summary>
    MagicStick3,

    /// <summary>Icone <c>Magnet</c> do contrato com o RVM.UI, desenhado com o <c>magnet</c> do Tabler.</summary>
    Magnet,

    /// <summary>Icone <c>MagnetWave</c> do contrato com o RVM.UI, desenhado com o <c>magnet</c> do Tabler.</summary>
    MagnetWave,

    /// <summary>Icone <c>Magnifer</c> do contrato com o RVM.UI, desenhado com o <c>search</c> do Tabler.</summary>
    Magnifer,

    /// <summary>Icone <c>MagniferBug</c> do contrato com o RVM.UI, desenhado com o <c>bug</c> do Tabler.</summary>
    MagniferBug,

    /// <summary>Icone <c>MagniferBugRounded</c> do contrato com o RVM.UI, desenhado com o <c>bug</c> do Tabler.</summary>
    MagniferBugRounded,

    /// <summary>Icone <c>MagniferMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>search</c> do Tabler.</summary>
    MagniferMinimalistic,

    /// <summary>Icone <c>MagniferRounded</c> do contrato com o RVM.UI, desenhado com o <c>search</c> do Tabler.</summary>
    MagniferRounded,

    /// <summary>Icone <c>MagniferZoomIn</c> do contrato com o RVM.UI, desenhado com o <c>zoom-in</c> do Tabler.</summary>
    MagniferZoomIn,

    /// <summary>Icone <c>MagniferZoomInMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>zoom-in</c> do Tabler.</summary>
    MagniferZoomInMinimalistic,

    /// <summary>Icone <c>MagniferZoomInRounded</c> do contrato com o RVM.UI, desenhado com o <c>zoom-in</c> do Tabler.</summary>
    MagniferZoomInRounded,

    /// <summary>Icone <c>MagniferZoomOut</c> do contrato com o RVM.UI, desenhado com o <c>zoom-out</c> do Tabler.</summary>
    MagniferZoomOut,

    /// <summary>Icone <c>MagniferZoomOutMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>zoom-out</c> do Tabler.</summary>
    MagniferZoomOutMinimalistic,

    /// <summary>Icone <c>MagniferZoomOutRounded</c> do contrato com o RVM.UI, desenhado com o <c>zoom-out</c> do Tabler.</summary>
    MagniferZoomOutRounded,

    /// <summary>Icone <c>MagnifierBugMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>bug</c> do Tabler.</summary>
    MagnifierBugMinimalistic,

    /// <summary>Icone <c>Mailbox</c> do contrato com o RVM.UI, desenhado com o <c>mailbox</c> do Tabler.</summary>
    Mailbox,

    /// <summary>Icone <c>Map</c> do contrato com o RVM.UI, desenhado com o <c>map</c> do Tabler.</summary>
    Map,

    /// <summary>Icone <c>MapArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-down</c> do Tabler.</summary>
    MapArrowDown,

    /// <summary>Icone <c>MapArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>arrow-left</c> do Tabler.</summary>
    MapArrowLeft,

    /// <summary>Icone <c>MapArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>arrow-right</c> do Tabler.</summary>
    MapArrowRight,

    /// <summary>Icone <c>MapArrowSquare</c> do contrato com o RVM.UI, desenhado com o <c>map-2</c> do Tabler.</summary>
    MapArrowSquare,

    /// <summary>Icone <c>MapArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-up</c> do Tabler.</summary>
    MapArrowUp,

    /// <summary>Icone <c>MapPoint</c> do contrato com o RVM.UI, desenhado com o <c>map-pin</c> do Tabler.</summary>
    MapPoint,

    /// <summary>Icone <c>MapPointAdd</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-plus</c> do Tabler.</summary>
    MapPointAdd,

    /// <summary>Icone <c>MapPointFavourite</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-heart</c> do Tabler.</summary>
    MapPointFavourite,

    /// <summary>Icone <c>MapPointHospital</c> do contrato com o RVM.UI, desenhado com o <c>building-hospital</c> do Tabler.</summary>
    MapPointHospital,

    /// <summary>Icone <c>MapPointRemove</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-minus</c> do Tabler.</summary>
    MapPointRemove,

    /// <summary>Icone <c>MapPointRotate</c> do contrato com o RVM.UI, desenhado com o <c>map-pin-cog</c> do Tabler.</summary>
    MapPointRotate,

    /// <summary>Icone <c>MapPointSchool</c> do contrato com o RVM.UI, desenhado com o <c>school</c> do Tabler.</summary>
    MapPointSchool,

    /// <summary>Icone <c>MapPointSearch</c> do contrato com o RVM.UI, desenhado com o <c>map-search</c> do Tabler.</summary>
    MapPointSearch,

    /// <summary>Icone <c>MapPointWave</c> do contrato com o RVM.UI, desenhado com o <c>map-pin</c> do Tabler.</summary>
    MapPointWave,

    /// <summary>Icone <c>MaskHapply</c> do contrato com o RVM.UI, desenhado com o <c>masks-theater</c> do Tabler.</summary>
    MaskHapply,

    /// <summary>Icone <c>MaskSad</c> do contrato com o RVM.UI, desenhado com o <c>mood-sad</c> do Tabler.</summary>
    MaskSad,

    /// <summary>Icone <c>Masks</c> do contrato com o RVM.UI, desenhado com o <c>masks-theater</c> do Tabler.</summary>
    Masks,

    /// <summary>Icone <c>Maximize</c> do contrato com o RVM.UI, desenhado com o <c>maximize</c> do Tabler.</summary>
    Maximize,

    /// <summary>Icone <c>MaximizeSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrows-maximize</c> do Tabler.</summary>
    MaximizeSquare,

    /// <summary>Icone <c>MaximizeSquare2</c> do contrato com o RVM.UI, desenhado com o <c>arrows-maximize</c> do Tabler.</summary>
    MaximizeSquare2,

    /// <summary>Icone <c>MaximizeSquare3</c> do contrato com o RVM.UI, desenhado com o <c>arrows-maximize</c> do Tabler.</summary>
    MaximizeSquare3,

    /// <summary>Icone <c>MaximizeSquareMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>arrows-maximize</c> do Tabler.</summary>
    MaximizeSquareMinimalistic,

    /// <summary>Icone <c>MedalRibbon</c> do contrato com o RVM.UI, desenhado com o <c>medal</c> do Tabler.</summary>
    MedalRibbon,

    /// <summary>Icone <c>MedalRibbonStar</c> do contrato com o RVM.UI, desenhado com o <c>medal</c> do Tabler.</summary>
    MedalRibbonStar,

    /// <summary>Icone <c>MedalRibbonsStar</c> do contrato com o RVM.UI, desenhado com o <c>medal-2</c> do Tabler.</summary>
    MedalRibbonsStar,

    /// <summary>Icone <c>MedalStar</c> do contrato com o RVM.UI, desenhado com o <c>award</c> do Tabler.</summary>
    MedalStar,

    /// <summary>Icone <c>MedalStarCircle</c> do contrato com o RVM.UI, desenhado com o <c>award</c> do Tabler.</summary>
    MedalStarCircle,

    /// <summary>Icone <c>MedalStarSquare</c> do contrato com o RVM.UI, desenhado com o <c>award</c> do Tabler.</summary>
    MedalStarSquare,

    /// <summary>Icone <c>MedicalKit</c> do contrato com o RVM.UI, desenhado com o <c>first-aid-kit</c> do Tabler.</summary>
    MedicalKit,

    /// <summary>Icone <c>Meditation</c> do contrato com o RVM.UI, desenhado com o <c>yoga</c> do Tabler.</summary>
    Meditation,

    /// <summary>Icone <c>MeditationRound</c> do contrato com o RVM.UI, desenhado com o <c>yoga</c> do Tabler.</summary>
    MeditationRound,

    /// <summary>Icone <c>Men</c> do contrato com o RVM.UI, desenhado com o <c>users</c> do Tabler.</summary>
    Men,

    /// <summary>Icone <c>MentionCircle</c> do contrato com o RVM.UI, desenhado com o <c>at</c> do Tabler.</summary>
    MentionCircle,

    /// <summary>Icone <c>MentionSquare</c> do contrato com o RVM.UI, desenhado com o <c>at</c> do Tabler.</summary>
    MentionSquare,

    /// <summary>Icone <c>MenuDots</c> do contrato com o RVM.UI, desenhado com o <c>dots-vertical</c> do Tabler.</summary>
    MenuDots,

    /// <summary>Icone <c>MenuDots2</c> do contrato com o RVM.UI, desenhado com o <c>dots</c> do Tabler.</summary>
    MenuDots2,

    /// <summary>Icone <c>MenuDotsCircle</c> do contrato com o RVM.UI, desenhado com o <c>dots-circle-horizontal</c> do Tabler.</summary>
    MenuDotsCircle,

    /// <summary>Icone <c>MenuDotsCircle2</c> do contrato com o RVM.UI, desenhado com o <c>dots-circle-horizontal</c> do Tabler.</summary>
    MenuDotsCircle2,

    /// <summary>Icone <c>MenuDotsSquare</c> do contrato com o RVM.UI, desenhado com o <c>dots</c> do Tabler.</summary>
    MenuDotsSquare,

    /// <summary>Icone <c>MenuDotsSquare2</c> do contrato com o RVM.UI, desenhado com o <c>dots</c> do Tabler.</summary>
    MenuDotsSquare2,

    /// <summary>Icone <c>Microphone</c> do contrato com o RVM.UI, desenhado com o <c>microphone</c> do Tabler.</summary>
    Microphone,

    /// <summary>Icone <c>Microphone2</c> do contrato com o RVM.UI, desenhado com o <c>microphone-2</c> do Tabler.</summary>
    Microphone2,

    /// <summary>Icone <c>Microphone3</c> do contrato com o RVM.UI, desenhado com o <c>microphone</c> do Tabler.</summary>
    Microphone3,

    /// <summary>Icone <c>MicrophoneLarge</c> do contrato com o RVM.UI, desenhado com o <c>microphone</c> do Tabler.</summary>
    MicrophoneLarge,

    /// <summary>Icone <c>Minimize</c> do contrato com o RVM.UI, desenhado com o <c>minimize</c> do Tabler.</summary>
    Minimize,

    /// <summary>Icone <c>MinimizeSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    MinimizeSquare,

    /// <summary>Icone <c>MinimizeSquare2</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    MinimizeSquare2,

    /// <summary>Icone <c>MinimizeSquare3</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    MinimizeSquare3,

    /// <summary>Icone <c>MinimizeSquareMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    MinimizeSquareMinimalistic,

    /// <summary>Icone <c>MinusCircle</c> do contrato com o RVM.UI, desenhado com o <c>circle-minus</c> do Tabler.</summary>
    MinusCircle,

    /// <summary>Icone <c>MinusSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-minus</c> do Tabler.</summary>
    MinusSquare,

    /// <summary>Icone <c>Mirror</c> do contrato com o RVM.UI, desenhado com o <c>flip-vertical</c> do Tabler.</summary>
    Mirror,

    /// <summary>Icone <c>MirrorEssentionalUi</c> do contrato com o RVM.UI, desenhado com o <c>flip-vertical</c> do Tabler.</summary>
    MirrorEssentionalUi,

    /// <summary>Icone <c>MirrorLeft</c> do contrato com o RVM.UI, desenhado com o <c>flip-horizontal</c> do Tabler.</summary>
    MirrorLeft,

    /// <summary>Icone <c>MirrorRight</c> do contrato com o RVM.UI, desenhado com o <c>flip-horizontal</c> do Tabler.</summary>
    MirrorRight,

    /// <summary>Icone <c>MoneyBag</c> do contrato com o RVM.UI, desenhado com o <c>moneybag</c> do Tabler.</summary>
    MoneyBag,

    /// <summary>Icone <c>Monitor</c> do contrato com o RVM.UI, desenhado com o <c>device-desktop</c> do Tabler.</summary>
    Monitor,

    /// <summary>Icone <c>MonitorCamera</c> do contrato com o RVM.UI, desenhado com o <c>device-desktop</c> do Tabler.</summary>
    MonitorCamera,

    /// <summary>Icone <c>MonitorSmartphone</c> do contrato com o RVM.UI, desenhado com o <c>devices</c> do Tabler.</summary>
    MonitorSmartphone,

    /// <summary>Icone <c>Moon</c> do contrato com o RVM.UI, desenhado com o <c>moon</c> do Tabler.</summary>
    Moon,

    /// <summary>Icone <c>MoonFog</c> do contrato com o RVM.UI, desenhado com o <c>moon-stars</c> do Tabler.</summary>
    MoonFog,

    /// <summary>Icone <c>MoonSleep</c> do contrato com o RVM.UI, desenhado com o <c>moon</c> do Tabler.</summary>
    MoonSleep,

    /// <summary>Icone <c>MoonStars</c> do contrato com o RVM.UI, desenhado com o <c>moon-stars</c> do Tabler.</summary>
    MoonStars,

    /// <summary>Icone <c>Mouse</c> do contrato com o RVM.UI, desenhado com o <c>mouse</c> do Tabler.</summary>
    Mouse,

    /// <summary>Icone <c>MouseCircle</c> do contrato com o RVM.UI, desenhado com o <c>mouse</c> do Tabler.</summary>
    MouseCircle,

    /// <summary>Icone <c>MouseMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>mouse</c> do Tabler.</summary>
    MouseMinimalistic,

    /// <summary>Icone <c>MultipleForwardLeft</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    MultipleForwardLeft,

    /// <summary>Icone <c>MultipleForwardRight</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    MultipleForwardRight,

    /// <summary>Icone <c>MusicLibrary</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicLibrary,

    /// <summary>Icone <c>MusicLibrary2</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicLibrary2,

    /// <summary>Icone <c>MusicNote</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicNote,

    /// <summary>Icone <c>MusicNote2</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicNote2,

    /// <summary>Icone <c>MusicNote3</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicNote3,

    /// <summary>Icone <c>MusicNote4</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicNote4,

    /// <summary>Icone <c>MusicNoteSlider</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    MusicNoteSlider,

    /// <summary>Icone <c>MusicNoteSlider2</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    MusicNoteSlider2,

    /// <summary>Icone <c>MusicNotes</c> do contrato com o RVM.UI, desenhado com o <c>music</c> do Tabler.</summary>
    MusicNotes,

    /// <summary>Icone <c>Muted</c> do contrato com o RVM.UI, desenhado com o <c>volume-off</c> do Tabler.</summary>
    Muted,

    /// <summary>Icone <c>N4k</c> do contrato com o RVM.UI, desenhado com o <c>badge-4k</c> do Tabler.</summary>
    N4k,

    /// <summary>Icone <c>Notebook</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    Notebook,

    /// <summary>Icone <c>Notebook2</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    Notebook2,

    /// <summary>Icone <c>Notebook2ElectronicDevices</c> do contrato com o RVM.UI, desenhado com o <c>device-laptop</c> do Tabler.</summary>
    Notebook2ElectronicDevices,

    /// <summary>Icone <c>NotebookBookmark</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    NotebookBookmark,

    /// <summary>Icone <c>NotebookElectronicDevices</c> do contrato com o RVM.UI, desenhado com o <c>device-laptop</c> do Tabler.</summary>
    NotebookElectronicDevices,

    /// <summary>Icone <c>NotebookMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    NotebookMinimalistic,

    /// <summary>Icone <c>NotebookMinimalisticElectronicDevices</c> do contrato com o RVM.UI, desenhado com o <c>device-laptop</c> do Tabler.</summary>
    NotebookMinimalisticElectronicDevices,

    /// <summary>Icone <c>NotebookNotes</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    NotebookNotes,

    /// <summary>Icone <c>NotebookSquare</c> do contrato com o RVM.UI, desenhado com o <c>notebook</c> do Tabler.</summary>
    NotebookSquare,

    /// <summary>Icone <c>Notes</c> do contrato com o RVM.UI, desenhado com o <c>notes</c> do Tabler.</summary>
    Notes,

    /// <summary>Icone <c>NotesMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>note</c> do Tabler.</summary>
    NotesMinimalistic,

    /// <summary>Icone <c>NotificationLinesRemove</c> do contrato com o RVM.UI, desenhado com o <c>bell-off</c> do Tabler.</summary>
    NotificationLinesRemove,

    /// <summary>Icone <c>NotificationRemove</c> do contrato com o RVM.UI, desenhado com o <c>bell-off</c> do Tabler.</summary>
    NotificationRemove,

    /// <summary>Icone <c>NotificationUnread</c> do contrato com o RVM.UI, desenhado com o <c>bell-ringing</c> do Tabler.</summary>
    NotificationUnread,

    /// <summary>Icone <c>NotificationUnreadLines</c> do contrato com o RVM.UI, desenhado com o <c>bell-ringing</c> do Tabler.</summary>
    NotificationUnreadLines,

    /// <summary>Icone <c>ObjectScan</c> do contrato com o RVM.UI, desenhado com o <c>object-scan</c> do Tabler.</summary>
    ObjectScan,

    /// <summary>Icone <c>OutgoingCall</c> do contrato com o RVM.UI, desenhado com o <c>phone-outgoing</c> do Tabler.</summary>
    OutgoingCall,

    /// <summary>Icone <c>OutgoingCallRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-outgoing</c> do Tabler.</summary>
    OutgoingCallRounded,

    /// <summary>Icone <c>OvenMitts</c> do contrato com o RVM.UI, desenhado com o <c>tools-kitchen-2</c> do Tabler.</summary>
    OvenMitts,

    /// <summary>Icone <c>OvenMittsMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>tools-kitchen-2</c> do Tabler.</summary>
    OvenMittsMinimalistic,

    /// <summary>Icone <c>PaintRoller</c> do contrato com o RVM.UI, desenhado com o <c>paint</c> do Tabler.</summary>
    PaintRoller,

    /// <summary>Icone <c>Palette</c> do contrato com o RVM.UI, desenhado com o <c>palette</c> do Tabler.</summary>
    Palette,

    /// <summary>Icone <c>PaletteRound</c> do contrato com o RVM.UI, desenhado com o <c>palette</c> do Tabler.</summary>
    PaletteRound,

    /// <summary>Icone <c>Panorama</c> do contrato com o RVM.UI, desenhado com o <c>panorama-horizontal</c> do Tabler.</summary>
    Panorama,

    /// <summary>Icone <c>PaperBin</c> do contrato com o RVM.UI, desenhado com o <c>trash</c> do Tabler.</summary>
    PaperBin,

    /// <summary>Icone <c>Paperclip</c> do contrato com o RVM.UI, desenhado com o <c>paperclip</c> do Tabler.</summary>
    Paperclip,

    /// <summary>Icone <c>Paperclip2</c> do contrato com o RVM.UI, desenhado com o <c>paperclip</c> do Tabler.</summary>
    Paperclip2,

    /// <summary>Icone <c>PaperclipRounded</c> do contrato com o RVM.UI, desenhado com o <c>paperclip</c> do Tabler.</summary>
    PaperclipRounded,

    /// <summary>Icone <c>PaperclipRounded2</c> do contrato com o RVM.UI, desenhado com o <c>paperclip</c> do Tabler.</summary>
    PaperclipRounded2,

    /// <summary>Icone <c>ParagraphSpacing</c> do contrato com o RVM.UI, desenhado com o <c>line-height</c> do Tabler.</summary>
    ParagraphSpacing,

    /// <summary>Icone <c>Passport</c> do contrato com o RVM.UI, desenhado com o <c>id</c> do Tabler.</summary>
    Passport,

    /// <summary>Icone <c>PassportMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>id</c> do Tabler.</summary>
    PassportMinimalistic,

    /// <summary>Icone <c>Password</c> do contrato com o RVM.UI, desenhado com o <c>password</c> do Tabler.</summary>
    Password,

    /// <summary>Icone <c>PasswordMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>password</c> do Tabler.</summary>
    PasswordMinimalistic,

    /// <summary>Icone <c>PasswordMinimalisticInput</c> do contrato com o RVM.UI, desenhado com o <c>password-user</c> do Tabler.</summary>
    PasswordMinimalisticInput,

    /// <summary>Icone <c>Pause</c> do contrato com o RVM.UI, desenhado com o <c>player-pause</c> do Tabler.</summary>
    Pause,

    /// <summary>Icone <c>PauseCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-pause</c> do Tabler.</summary>
    PauseCircle,

    /// <summary>Icone <c>Paw</c> do contrato com o RVM.UI, desenhado com o <c>paw</c> do Tabler.</summary>
    Paw,

    /// <summary>Icone <c>Pen</c> do contrato com o RVM.UI, desenhado com o <c>pencil</c> do Tabler.</summary>
    Pen,

    /// <summary>Icone <c>Pen2</c> do contrato com o RVM.UI, desenhado com o <c>pencil</c> do Tabler.</summary>
    Pen2,

    /// <summary>Icone <c>PenNewRound</c> do contrato com o RVM.UI, desenhado com o <c>pencil-plus</c> do Tabler.</summary>
    PenNewRound,

    /// <summary>Icone <c>PenNewSquare</c> do contrato com o RVM.UI, desenhado com o <c>edit</c> do Tabler.</summary>
    PenNewSquare,

    /// <summary>Icone <c>PeopleNearby</c> do contrato com o RVM.UI, desenhado com o <c>users-group</c> do Tabler.</summary>
    PeopleNearby,

    /// <summary>Icone <c>Perfume</c> do contrato com o RVM.UI, desenhado com o <c>perfume</c> do Tabler.</summary>
    Perfume,

    /// <summary>Icone <c>PhoneCalling</c> do contrato com o RVM.UI, desenhado com o <c>phone-calling</c> do Tabler.</summary>
    PhoneCalling,

    /// <summary>Icone <c>PhoneCallingRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone-call</c> do Tabler.</summary>
    PhoneCallingRounded,

    /// <summary>Icone <c>PhoneRounded</c> do contrato com o RVM.UI, desenhado com o <c>phone</c> do Tabler.</summary>
    PhoneRounded,

    /// <summary>Icone <c>PieChart</c> do contrato com o RVM.UI, desenhado com o <c>chart-pie</c> do Tabler.</summary>
    PieChart,

    /// <summary>Icone <c>PieChart2</c> do contrato com o RVM.UI, desenhado com o <c>chart-pie-2</c> do Tabler.</summary>
    PieChart2,

    /// <summary>Icone <c>PieChart3</c> do contrato com o RVM.UI, desenhado com o <c>chart-pie-3</c> do Tabler.</summary>
    PieChart3,

    /// <summary>Icone <c>Pill</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    Pill,

    /// <summary>Icone <c>Pills</c> do contrato com o RVM.UI, desenhado com o <c>pills</c> do Tabler.</summary>
    Pills,

    /// <summary>Icone <c>Pills2</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    Pills2,

    /// <summary>Icone <c>Pills3</c> do contrato com o RVM.UI, desenhado com o <c>pill</c> do Tabler.</summary>
    Pills3,

    /// <summary>Icone <c>Pin</c> do contrato com o RVM.UI, desenhado com o <c>pin</c> do Tabler.</summary>
    Pin,

    /// <summary>Icone <c>PinCircle</c> do contrato com o RVM.UI, desenhado com o <c>pin</c> do Tabler.</summary>
    PinCircle,

    /// <summary>Icone <c>PinList</c> do contrato com o RVM.UI, desenhado com o <c>pin</c> do Tabler.</summary>
    PinList,

    /// <summary>Icone <c>Pip</c> do contrato com o RVM.UI, desenhado com o <c>picture-in-picture</c> do Tabler.</summary>
    Pip,

    /// <summary>Icone <c>Pip2</c> do contrato com o RVM.UI, desenhado com o <c>picture-in-picture-on</c> do Tabler.</summary>
    Pip2,

    /// <summary>Icone <c>Pipette</c> do contrato com o RVM.UI, desenhado com o <c>color-picker</c> do Tabler.</summary>
    Pipette,

    /// <summary>Icone <c>Plain</c> do contrato com o RVM.UI, desenhado com o <c>send</c> do Tabler.</summary>
    Plain,

    /// <summary>Icone <c>Plain2</c> do contrato com o RVM.UI, desenhado com o <c>send</c> do Tabler.</summary>
    Plain2,

    /// <summary>Icone <c>Plain3</c> do contrato com o RVM.UI, desenhado com o <c>plane</c> do Tabler.</summary>
    Plain3,

    /// <summary>Icone <c>Planet</c> do contrato com o RVM.UI, desenhado com o <c>planet</c> do Tabler.</summary>
    Planet,

    /// <summary>Icone <c>Planet2</c> do contrato com o RVM.UI, desenhado com o <c>planet</c> do Tabler.</summary>
    Planet2,

    /// <summary>Icone <c>Planet3</c> do contrato com o RVM.UI, desenhado com o <c>planet</c> do Tabler.</summary>
    Planet3,

    /// <summary>Icone <c>Planet4</c> do contrato com o RVM.UI, desenhado com o <c>planet</c> do Tabler.</summary>
    Planet4,

    /// <summary>Icone <c>Plate</c> do contrato com o RVM.UI, desenhado com o <c>tools-kitchen-2</c> do Tabler.</summary>
    Plate,

    /// <summary>Icone <c>Play</c> do contrato com o RVM.UI, desenhado com o <c>player-play</c> do Tabler.</summary>
    Play,

    /// <summary>Icone <c>PlayCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-play</c> do Tabler.</summary>
    PlayCircle,

    /// <summary>Icone <c>PlayStream</c> do contrato com o RVM.UI, desenhado com o <c>broadcast</c> do Tabler.</summary>
    PlayStream,

    /// <summary>Icone <c>PlaybackSpeed</c> do contrato com o RVM.UI, desenhado com o <c>brand-speedtest</c> do Tabler.</summary>
    PlaybackSpeed,

    /// <summary>Icone <c>Playlist</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    Playlist,

    /// <summary>Icone <c>Playlist2</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    Playlist2,

    /// <summary>Icone <c>PlaylistMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    PlaylistMinimalistic,

    /// <summary>Icone <c>PlaylistMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    PlaylistMinimalistic2,

    /// <summary>Icone <c>PlaylistMinimalistic3</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    PlaylistMinimalistic3,

    /// <summary>Icone <c>PlugCircle</c> do contrato com o RVM.UI, desenhado com o <c>plug</c> do Tabler.</summary>
    PlugCircle,

    /// <summary>Icone <c>PlusMinus</c> do contrato com o RVM.UI, desenhado com o <c>plus-minus</c> do Tabler.</summary>
    PlusMinus,

    /// <summary>Icone <c>Podcast</c> do contrato com o RVM.UI, desenhado com o <c>microphone-2</c> do Tabler.</summary>
    Podcast,

    /// <summary>Icone <c>PointOnMap</c> do contrato com o RVM.UI, desenhado com o <c>map-pin</c> do Tabler.</summary>
    PointOnMap,

    /// <summary>Icone <c>PointOnMapPerspective</c> do contrato com o RVM.UI, desenhado com o <c>map-pin</c> do Tabler.</summary>
    PointOnMapPerspective,

    /// <summary>Icone <c>PostsCarouselHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>carousel-horizontal</c> do Tabler.</summary>
    PostsCarouselHorizontal,

    /// <summary>Icone <c>PostsCarouselVertical</c> do contrato com o RVM.UI, desenhado com o <c>carousel-vertical</c> do Tabler.</summary>
    PostsCarouselVertical,

    /// <summary>Icone <c>Power</c> do contrato com o RVM.UI, desenhado com o <c>power</c> do Tabler.</summary>
    Power,

    /// <summary>Icone <c>PresentationGraph</c> do contrato com o RVM.UI, desenhado com o <c>presentation-analytics</c> do Tabler.</summary>
    PresentationGraph,

    /// <summary>Icone <c>Printer2</c> do contrato com o RVM.UI, desenhado com o <c>printer</c> do Tabler.</summary>
    Printer2,

    /// <summary>Icone <c>PrinterMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>printer</c> do Tabler.</summary>
    PrinterMinimalistic,

    /// <summary>Icone <c>Programming</c> do contrato com o RVM.UI, desenhado com o <c>code</c> do Tabler.</summary>
    Programming,

    /// <summary>Icone <c>Projector</c> do contrato com o RVM.UI, desenhado com o <c>device-projector</c> do Tabler.</summary>
    Projector,

    /// <summary>Icone <c>Property</c> do contrato com o RVM.UI, desenhado com o <c>building-estate</c> do Tabler.</summary>
    Property,

    /// <summary>Icone <c>Pulse</c> do contrato com o RVM.UI, desenhado com o <c>activity</c> do Tabler.</summary>
    Pulse,

    /// <summary>Icone <c>Pulse2</c> do contrato com o RVM.UI, desenhado com o <c>activity-heartbeat</c> do Tabler.</summary>
    Pulse2,

    /// <summary>Icone <c>QrCode</c> do contrato com o RVM.UI, desenhado com o <c>qrcode</c> do Tabler.</summary>
    QrCode,

    /// <summary>Icone <c>QuestionCircle</c> do contrato com o RVM.UI, desenhado com o <c>help-circle</c> do Tabler.</summary>
    QuestionCircle,

    /// <summary>Icone <c>QuestionSquare</c> do contrato com o RVM.UI, desenhado com o <c>help-square</c> do Tabler.</summary>
    QuestionSquare,

    /// <summary>Icone <c>QuitFullScreen</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    QuitFullScreen,

    /// <summary>Icone <c>QuitFullScreenCircle</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    QuitFullScreenCircle,

    /// <summary>Icone <c>QuitFullScreenSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrows-minimize</c> do Tabler.</summary>
    QuitFullScreenSquare,

    /// <summary>Icone <c>QuitPip</c> do contrato com o RVM.UI, desenhado com o <c>picture-in-picture-off</c> do Tabler.</summary>
    QuitPip,

    /// <summary>Icone <c>Radar</c> do contrato com o RVM.UI, desenhado com o <c>radar</c> do Tabler.</summary>
    Radar,

    /// <summary>Icone <c>Radar2</c> do contrato com o RVM.UI, desenhado com o <c>radar-2</c> do Tabler.</summary>
    Radar2,

    /// <summary>Icone <c>RadialBlur</c> do contrato com o RVM.UI, desenhado com o <c>blur</c> do Tabler.</summary>
    RadialBlur,

    /// <summary>Icone <c>Radio</c> do contrato com o RVM.UI, desenhado com o <c>radio</c> do Tabler.</summary>
    Radio,

    /// <summary>Icone <c>RadioMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>radio</c> do Tabler.</summary>
    RadioMinimalistic,

    /// <summary>Icone <c>Ranking</c> do contrato com o RVM.UI, desenhado com o <c>trophy</c> do Tabler.</summary>
    Ranking,

    /// <summary>Icone <c>ReciveSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    ReciveSquare,

    /// <summary>Icone <c>ReciveTwiceSquare</c> do contrato com o RVM.UI, desenhado com o <c>transfer-in</c> do Tabler.</summary>
    ReciveTwiceSquare,

    /// <summary>Icone <c>Record</c> do contrato com o RVM.UI, desenhado com o <c>record-mail</c> do Tabler.</summary>
    Record,

    /// <summary>Icone <c>RecordCall</c> do contrato com o RVM.UI, desenhado com o <c>phone-calling</c> do Tabler.</summary>
    RecordCall,

    /// <summary>Icone <c>RecordCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-record</c> do Tabler.</summary>
    RecordCircle,

    /// <summary>Icone <c>RecordCircleCall</c> do contrato com o RVM.UI, desenhado com o <c>phone-calling</c> do Tabler.</summary>
    RecordCircleCall,

    /// <summary>Icone <c>RecordSquare</c> do contrato com o RVM.UI, desenhado com o <c>player-record</c> do Tabler.</summary>
    RecordSquare,

    /// <summary>Icone <c>RecordVideoAudioSound</c> do contrato com o RVM.UI, desenhado com o <c>video</c> do Tabler.</summary>
    RecordVideoAudioSound,

    /// <summary>Icone <c>Reel</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    Reel,

    /// <summary>Icone <c>Reel2</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    Reel2,

    /// <summary>Icone <c>RefreshCircle</c> do contrato com o RVM.UI, desenhado com o <c>refresh</c> do Tabler.</summary>
    RefreshCircle,

    /// <summary>Icone <c>RefreshCircle2</c> do contrato com o RVM.UI, desenhado com o <c>refresh</c> do Tabler.</summary>
    RefreshCircle2,

    /// <summary>Icone <c>RefreshSquare</c> do contrato com o RVM.UI, desenhado com o <c>refresh</c> do Tabler.</summary>
    RefreshSquare,

    /// <summary>Icone <c>RefreshSquare2</c> do contrato com o RVM.UI, desenhado com o <c>refresh</c> do Tabler.</summary>
    RefreshSquare2,

    /// <summary>Icone <c>RemoteController</c> do contrato com o RVM.UI, desenhado com o <c>device-remote</c> do Tabler.</summary>
    RemoteController,

    /// <summary>Icone <c>RemoteController2</c> do contrato com o RVM.UI, desenhado com o <c>device-remote</c> do Tabler.</summary>
    RemoteController2,

    /// <summary>Icone <c>RemoteControllerMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>device-remote</c> do Tabler.</summary>
    RemoteControllerMinimalistic,

    /// <summary>Icone <c>RemoveFolder</c> do contrato com o RVM.UI, desenhado com o <c>folder-minus</c> do Tabler.</summary>
    RemoveFolder,

    /// <summary>Icone <c>Reorder</c> do contrato com o RVM.UI, desenhado com o <c>reorder</c> do Tabler.</summary>
    Reorder,

    /// <summary>Icone <c>ReorderArrowsAction</c> do contrato com o RVM.UI, desenhado com o <c>arrows-sort</c> do Tabler.</summary>
    ReorderArrowsAction,

    /// <summary>Icone <c>Repeat</c> do contrato com o RVM.UI, desenhado com o <c>repeat</c> do Tabler.</summary>
    Repeat,

    /// <summary>Icone <c>RepeatOne</c> do contrato com o RVM.UI, desenhado com o <c>repeat-once</c> do Tabler.</summary>
    RepeatOne,

    /// <summary>Icone <c>RepeatOneMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>repeat-once</c> do Tabler.</summary>
    RepeatOneMinimalistic,

    /// <summary>Icone <c>Reply</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    Reply,

    /// <summary>Icone <c>Reply2</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    Reply2,

    /// <summary>Icone <c>Restart</c> do contrato com o RVM.UI, desenhado com o <c>rotate</c> do Tabler.</summary>
    Restart,

    /// <summary>Icone <c>Revote</c> do contrato com o RVM.UI, desenhado com o <c>refresh</c> do Tabler.</summary>
    Revote,

    /// <summary>Icone <c>Rewind10SecondsBack</c> do contrato com o RVM.UI, desenhado com o <c>rewind-backward-10</c> do Tabler.</summary>
    Rewind10SecondsBack,

    /// <summary>Icone <c>Rewind10SecondsForward</c> do contrato com o RVM.UI, desenhado com o <c>rewind-forward-10</c> do Tabler.</summary>
    Rewind10SecondsForward,

    /// <summary>Icone <c>Rewind15SecondsBack</c> do contrato com o RVM.UI, desenhado com o <c>rewind-backward-15</c> do Tabler.</summary>
    Rewind15SecondsBack,

    /// <summary>Icone <c>Rewind15SecondsForward</c> do contrato com o RVM.UI, desenhado com o <c>rewind-forward-15</c> do Tabler.</summary>
    Rewind15SecondsForward,

    /// <summary>Icone <c>Rewind5SecondsBack</c> do contrato com o RVM.UI, desenhado com o <c>rewind-backward-5</c> do Tabler.</summary>
    Rewind5SecondsBack,

    /// <summary>Icone <c>Rewind5SecondsForward</c> do contrato com o RVM.UI, desenhado com o <c>rewind-forward-5</c> do Tabler.</summary>
    Rewind5SecondsForward,

    /// <summary>Icone <c>RewindBack</c> do contrato com o RVM.UI, desenhado com o <c>player-track-prev</c> do Tabler.</summary>
    RewindBack,

    /// <summary>Icone <c>RewindBackCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-track-prev</c> do Tabler.</summary>
    RewindBackCircle,

    /// <summary>Icone <c>RewindForward</c> do contrato com o RVM.UI, desenhado com o <c>player-track-next</c> do Tabler.</summary>
    RewindForward,

    /// <summary>Icone <c>RewindForwardCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-track-next</c> do Tabler.</summary>
    RewindForwardCircle,

    /// <summary>Icone <c>Rocket</c> do contrato com o RVM.UI, desenhado com o <c>rocket</c> do Tabler.</summary>
    Rocket,

    /// <summary>Icone <c>Rocket2</c> do contrato com o RVM.UI, desenhado com o <c>rocket</c> do Tabler.</summary>
    Rocket2,

    /// <summary>Icone <c>RollingPin</c> do contrato com o RVM.UI, desenhado com o <c>tools-kitchen-2</c> do Tabler.</summary>
    RollingPin,

    /// <summary>Icone <c>RoundAltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevron-down</c> do Tabler.</summary>
    RoundAltArrowDown,

    /// <summary>Icone <c>RoundAltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevron-left</c> do Tabler.</summary>
    RoundAltArrowLeft,

    /// <summary>Icone <c>RoundAltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevron-right</c> do Tabler.</summary>
    RoundAltArrowRight,

    /// <summary>Icone <c>RoundAltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevron-up</c> do Tabler.</summary>
    RoundAltArrowUp,

    /// <summary>Icone <c>RoundArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-down</c> do Tabler.</summary>
    RoundArrowDown,

    /// <summary>Icone <c>RoundArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-left</c> do Tabler.</summary>
    RoundArrowLeft,

    /// <summary>Icone <c>RoundArrowLeftDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-down-left</c> do Tabler.</summary>
    RoundArrowLeftDown,

    /// <summary>Icone <c>RoundArrowLeftUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-up-left</c> do Tabler.</summary>
    RoundArrowLeftUp,

    /// <summary>Icone <c>RoundArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-right</c> do Tabler.</summary>
    RoundArrowRight,

    /// <summary>Icone <c>RoundArrowRightDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-down-right</c> do Tabler.</summary>
    RoundArrowRightDown,

    /// <summary>Icone <c>RoundArrowRightUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-up-right</c> do Tabler.</summary>
    RoundArrowRightUp,

    /// <summary>Icone <c>RoundArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-arrow-up</c> do Tabler.</summary>
    RoundArrowUp,

    /// <summary>Icone <c>RoundDoubleAltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevrons-down</c> do Tabler.</summary>
    RoundDoubleAltArrowDown,

    /// <summary>Icone <c>RoundDoubleAltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevrons-left</c> do Tabler.</summary>
    RoundDoubleAltArrowLeft,

    /// <summary>Icone <c>RoundDoubleAltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevrons-right</c> do Tabler.</summary>
    RoundDoubleAltArrowRight,

    /// <summary>Icone <c>RoundDoubleAltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>circle-chevrons-up</c> do Tabler.</summary>
    RoundDoubleAltArrowUp,

    /// <summary>Icone <c>RoundGraph</c> do contrato com o RVM.UI, desenhado com o <c>chart-donut</c> do Tabler.</summary>
    RoundGraph,

    /// <summary>Icone <c>RoundSortHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-horizontal</c> do Tabler.</summary>
    RoundSortHorizontal,

    /// <summary>Icone <c>RoundSortVertical</c> do contrato com o RVM.UI, desenhado com o <c>arrows-vertical</c> do Tabler.</summary>
    RoundSortVertical,

    /// <summary>Icone <c>RoundTransferDiagonal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-diagonal</c> do Tabler.</summary>
    RoundTransferDiagonal,

    /// <summary>Icone <c>RoundTransferHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-exchange</c> do Tabler.</summary>
    RoundTransferHorizontal,

    /// <summary>Icone <c>RoundTransferVertical</c> do contrato com o RVM.UI, desenhado com o <c>arrows-exchange-2</c> do Tabler.</summary>
    RoundTransferVertical,

    /// <summary>Icone <c>Route</c> do contrato com o RVM.UI, desenhado com o <c>route</c> do Tabler.</summary>
    Route,

    /// <summary>Icone <c>Routing</c> do contrato com o RVM.UI, desenhado com o <c>route</c> do Tabler.</summary>
    Routing,

    /// <summary>Icone <c>Routing2</c> do contrato com o RVM.UI, desenhado com o <c>route</c> do Tabler.</summary>
    Routing2,

    /// <summary>Icone <c>Routing3</c> do contrato com o RVM.UI, desenhado com o <c>route-2</c> do Tabler.</summary>
    Routing3,

    /// <summary>Icone <c>Ruble</c> do contrato com o RVM.UI, desenhado com o <c>currency-ruble</c> do Tabler.</summary>
    Ruble,

    /// <summary>Icone <c>Rugby</c> do contrato com o RVM.UI, desenhado com o <c>rugby</c> do Tabler.</summary>
    Rugby,

    /// <summary>Icone <c>Ruler</c> do contrato com o RVM.UI, desenhado com o <c>ruler</c> do Tabler.</summary>
    Ruler,

    /// <summary>Icone <c>RulerAngular</c> do contrato com o RVM.UI, desenhado com o <c>ruler-2</c> do Tabler.</summary>
    RulerAngular,

    /// <summary>Icone <c>RulerCrossPen</c> do contrato com o RVM.UI, desenhado com o <c>ruler-measure</c> do Tabler.</summary>
    RulerCrossPen,

    /// <summary>Icone <c>RulerPen</c> do contrato com o RVM.UI, desenhado com o <c>ruler</c> do Tabler.</summary>
    RulerPen,

    /// <summary>Icone <c>Running</c> do contrato com o RVM.UI, desenhado com o <c>run</c> do Tabler.</summary>
    Running,

    /// <summary>Icone <c>Running2</c> do contrato com o RVM.UI, desenhado com o <c>run</c> do Tabler.</summary>
    Running2,

    /// <summary>Icone <c>RunningRound</c> do contrato com o RVM.UI, desenhado com o <c>run</c> do Tabler.</summary>
    RunningRound,

    /// <summary>Icone <c>SadCircle</c> do contrato com o RVM.UI, desenhado com o <c>mood-sad</c> do Tabler.</summary>
    SadCircle,

    /// <summary>Icone <c>SadSquare</c> do contrato com o RVM.UI, desenhado com o <c>mood-sad</c> do Tabler.</summary>
    SadSquare,

    /// <summary>Icone <c>Safe2</c> do contrato com o RVM.UI, desenhado com o <c>vault</c> do Tabler.</summary>
    Safe2,

    /// <summary>Icone <c>SafeCircle</c> do contrato com o RVM.UI, desenhado com o <c>vault</c> do Tabler.</summary>
    SafeCircle,

    /// <summary>Icone <c>SafeSquare</c> do contrato com o RVM.UI, desenhado com o <c>vault</c> do Tabler.</summary>
    SafeSquare,

    /// <summary>Icone <c>Sale</c> do contrato com o RVM.UI, desenhado com o <c>discount</c> do Tabler.</summary>
    Sale,

    /// <summary>Icone <c>SaleSquare</c> do contrato com o RVM.UI, desenhado com o <c>discount</c> do Tabler.</summary>
    SaleSquare,

    /// <summary>Icone <c>Satellite</c> do contrato com o RVM.UI, desenhado com o <c>satellite</c> do Tabler.</summary>
    Satellite,

    /// <summary>Icone <c>Scale</c> do contrato com o RVM.UI, desenhado com o <c>scale</c> do Tabler.</summary>
    Scale,

    /// <summary>Icone <c>Scanner</c> do contrato com o RVM.UI, desenhado com o <c>scan</c> do Tabler.</summary>
    Scanner,

    /// <summary>Icone <c>Scanner2</c> do contrato com o RVM.UI, desenhado com o <c>scan</c> do Tabler.</summary>
    Scanner2,

    /// <summary>Icone <c>Scissors</c> do contrato com o RVM.UI, desenhado com o <c>scissors</c> do Tabler.</summary>
    Scissors,

    /// <summary>Icone <c>ScissorsSquare</c> do contrato com o RVM.UI, desenhado com o <c>scissors</c> do Tabler.</summary>
    ScissorsSquare,

    /// <summary>Icone <c>Scooter</c> do contrato com o RVM.UI, desenhado com o <c>scooter</c> do Tabler.</summary>
    Scooter,

    /// <summary>Icone <c>ScreenShare</c> do contrato com o RVM.UI, desenhado com o <c>screen-share</c> do Tabler.</summary>
    ScreenShare,

    /// <summary>Icone <c>Screencast</c> do contrato com o RVM.UI, desenhado com o <c>screen-share</c> do Tabler.</summary>
    Screencast,

    /// <summary>Icone <c>Screencast2</c> do contrato com o RVM.UI, desenhado com o <c>screen-share</c> do Tabler.</summary>
    Screencast2,

    /// <summary>Icone <c>SdCard</c> do contrato com o RVM.UI, desenhado com o <c>device-sd-card</c> do Tabler.</summary>
    SdCard,

    /// <summary>Icone <c>SendSquare</c> do contrato com o RVM.UI, desenhado com o <c>send</c> do Tabler.</summary>
    SendSquare,

    /// <summary>Icone <c>SendTwiceSquare</c> do contrato com o RVM.UI, desenhado com o <c>send</c> do Tabler.</summary>
    SendTwiceSquare,

    /// <summary>Icone <c>Server</c> do contrato com o RVM.UI, desenhado com o <c>server</c> do Tabler.</summary>
    Server,

    /// <summary>Icone <c>Server2</c> do contrato com o RVM.UI, desenhado com o <c>server-2</c> do Tabler.</summary>
    Server2,

    /// <summary>Icone <c>ServerMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>server</c> do Tabler.</summary>
    ServerMinimalistic,

    /// <summary>Icone <c>ServerPath</c> do contrato com o RVM.UI, desenhado com o <c>server-cog</c> do Tabler.</summary>
    ServerPath,

    /// <summary>Icone <c>ServerSquare</c> do contrato com o RVM.UI, desenhado com o <c>server</c> do Tabler.</summary>
    ServerSquare,

    /// <summary>Icone <c>ServerSquareCloud</c> do contrato com o RVM.UI, desenhado com o <c>server-2</c> do Tabler.</summary>
    ServerSquareCloud,

    /// <summary>Icone <c>ServerSquareUpdate</c> do contrato com o RVM.UI, desenhado com o <c>server-cog</c> do Tabler.</summary>
    ServerSquareUpdate,

    /// <summary>Icone <c>SettingsMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>settings</c> do Tabler.</summary>
    SettingsMinimalistic,

    /// <summary>Icone <c>Share</c> do contrato com o RVM.UI, desenhado com o <c>share</c> do Tabler.</summary>
    Share,

    /// <summary>Icone <c>ShareCircle</c> do contrato com o RVM.UI, desenhado com o <c>share</c> do Tabler.</summary>
    ShareCircle,

    /// <summary>Icone <c>Shield</c> do contrato com o RVM.UI, desenhado com o <c>shield</c> do Tabler.</summary>
    Shield,

    /// <summary>Icone <c>ShieldCheck</c> do contrato com o RVM.UI, desenhado com o <c>shield-check</c> do Tabler.</summary>
    ShieldCheck,

    /// <summary>Icone <c>ShieldCross</c> do contrato com o RVM.UI, desenhado com o <c>shield-x</c> do Tabler.</summary>
    ShieldCross,

    /// <summary>Icone <c>ShieldKeyhole</c> do contrato com o RVM.UI, desenhado com o <c>shield-lock</c> do Tabler.</summary>
    ShieldKeyhole,

    /// <summary>Icone <c>ShieldKeyholeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>shield-lock</c> do Tabler.</summary>
    ShieldKeyholeMinimalistic,

    /// <summary>Icone <c>ShieldMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>shield</c> do Tabler.</summary>
    ShieldMinimalistic,

    /// <summary>Icone <c>ShieldMinus</c> do contrato com o RVM.UI, desenhado com o <c>shield-minus</c> do Tabler.</summary>
    ShieldMinus,

    /// <summary>Icone <c>ShieldNetwork</c> do contrato com o RVM.UI, desenhado com o <c>shield-lock</c> do Tabler.</summary>
    ShieldNetwork,

    /// <summary>Icone <c>ShieldPlus</c> do contrato com o RVM.UI, desenhado com o <c>shield-plus</c> do Tabler.</summary>
    ShieldPlus,

    /// <summary>Icone <c>ShieldStar</c> do contrato com o RVM.UI, desenhado com o <c>shield-star</c> do Tabler.</summary>
    ShieldStar,

    /// <summary>Icone <c>ShieldUp</c> do contrato com o RVM.UI, desenhado com o <c>shield-up</c> do Tabler.</summary>
    ShieldUp,

    /// <summary>Icone <c>ShieldUser</c> do contrato com o RVM.UI, desenhado com o <c>user-shield</c> do Tabler.</summary>
    ShieldUser,

    /// <summary>Icone <c>ShieldWarning</c> do contrato com o RVM.UI, desenhado com o <c>shield-exclamation</c> do Tabler.</summary>
    ShieldWarning,

    /// <summary>Icone <c>ShockAbsorber</c> do contrato com o RVM.UI, desenhado com o <c>car-crash</c> do Tabler.</summary>
    ShockAbsorber,

    /// <summary>Icone <c>Shop</c> do contrato com o RVM.UI, desenhado com o <c>building-store</c> do Tabler.</summary>
    Shop,

    /// <summary>Icone <c>Shop2</c> do contrato com o RVM.UI, desenhado com o <c>building-store</c> do Tabler.</summary>
    Shop2,

    /// <summary>Icone <c>ShopMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>building-store</c> do Tabler.</summary>
    ShopMinimalistic,

    /// <summary>Icone <c>Shuffle</c> do contrato com o RVM.UI, desenhado com o <c>arrows-shuffle</c> do Tabler.</summary>
    Shuffle,

    /// <summary>Icone <c>SidebarCode</c> do contrato com o RVM.UI, desenhado com o <c>layout-sidebar</c> do Tabler.</summary>
    SidebarCode,

    /// <summary>Icone <c>SidebarMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>layout-sidebar</c> do Tabler.</summary>
    SidebarMinimalistic,

    /// <summary>Icone <c>Siderbar</c> do contrato com o RVM.UI, desenhado com o <c>layout-sidebar</c> do Tabler.</summary>
    Siderbar,

    /// <summary>Icone <c>Signpost</c> do contrato com o RVM.UI, desenhado com o <c>sign-right</c> do Tabler.</summary>
    Signpost,

    /// <summary>Icone <c>Signpost2</c> do contrato com o RVM.UI, desenhado com o <c>sign-left</c> do Tabler.</summary>
    Signpost2,

    /// <summary>Icone <c>SimCard</c> do contrato com o RVM.UI, desenhado com o <c>device-sim</c> do Tabler.</summary>
    SimCard,

    /// <summary>Icone <c>SimCardMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>device-sim</c> do Tabler.</summary>
    SimCardMinimalistic,

    /// <summary>Icone <c>SimCards</c> do contrato com o RVM.UI, desenhado com o <c>device-sim-2</c> do Tabler.</summary>
    SimCards,

    /// <summary>Icone <c>Siren</c> do contrato com o RVM.UI, desenhado com o <c>urgent</c> do Tabler.</summary>
    Siren,

    /// <summary>Icone <c>SirenRounded</c> do contrato com o RVM.UI, desenhado com o <c>urgent</c> do Tabler.</summary>
    SirenRounded,

    /// <summary>Icone <c>Skateboard</c> do contrato com o RVM.UI, desenhado com o <c>skateboard</c> do Tabler.</summary>
    Skateboard,

    /// <summary>Icone <c>Skateboarding</c> do contrato com o RVM.UI, desenhado com o <c>skateboarding</c> do Tabler.</summary>
    Skateboarding,

    /// <summary>Icone <c>SkateboardingRound</c> do contrato com o RVM.UI, desenhado com o <c>skateboard</c> do Tabler.</summary>
    SkateboardingRound,

    /// <summary>Icone <c>SkipNext</c> do contrato com o RVM.UI, desenhado com o <c>player-skip-forward</c> do Tabler.</summary>
    SkipNext,

    /// <summary>Icone <c>SkipPrevious</c> do contrato com o RVM.UI, desenhado com o <c>player-skip-back</c> do Tabler.</summary>
    SkipPrevious,

    /// <summary>Icone <c>Skirt</c> do contrato com o RVM.UI, desenhado com o <c>hanger</c> do Tabler.</summary>
    Skirt,

    /// <summary>Icone <c>SlashCircle</c> do contrato com o RVM.UI, desenhado com o <c>forbid</c> do Tabler.</summary>
    SlashCircle,

    /// <summary>Icone <c>SlashSquare</c> do contrato com o RVM.UI, desenhado com o <c>square-forbid</c> do Tabler.</summary>
    SlashSquare,

    /// <summary>Icone <c>Sledgehammer</c> do contrato com o RVM.UI, desenhado com o <c>hammer</c> do Tabler.</summary>
    Sledgehammer,

    /// <summary>Icone <c>SleepingCircle</c> do contrato com o RVM.UI, desenhado com o <c>zzz</c> do Tabler.</summary>
    SleepingCircle,

    /// <summary>Icone <c>SleepingSquare</c> do contrato com o RVM.UI, desenhado com o <c>zzz</c> do Tabler.</summary>
    SleepingSquare,

    /// <summary>Icone <c>SliderHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>adjustments-horizontal</c> do Tabler.</summary>
    SliderHorizontal,

    /// <summary>Icone <c>SliderMinimalisticHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>adjustments-horizontal</c> do Tabler.</summary>
    SliderMinimalisticHorizontal,

    /// <summary>Icone <c>SliderVertical</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    SliderVertical,

    /// <summary>Icone <c>SliderVerticalMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    SliderVerticalMinimalistic,

    /// <summary>Icone <c>SmartHome</c> do contrato com o RVM.UI, desenhado com o <c>smart-home</c> do Tabler.</summary>
    SmartHome,

    /// <summary>Icone <c>SmartHomeAngle</c> do contrato com o RVM.UI, desenhado com o <c>home</c> do Tabler.</summary>
    SmartHomeAngle,

    /// <summary>Icone <c>SmartSpeaker</c> do contrato com o RVM.UI, desenhado com o <c>device-speaker</c> do Tabler.</summary>
    SmartSpeaker,

    /// <summary>Icone <c>SmartSpeaker2</c> do contrato com o RVM.UI, desenhado com o <c>device-speaker</c> do Tabler.</summary>
    SmartSpeaker2,

    /// <summary>Icone <c>SmartSpeakerMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>device-speaker</c> do Tabler.</summary>
    SmartSpeakerMinimalistic,

    /// <summary>Icone <c>SmartVacuumCleaner</c> do contrato com o RVM.UI, desenhado com o <c>vacuum-cleaner</c> do Tabler.</summary>
    SmartVacuumCleaner,

    /// <summary>Icone <c>SmartVacuumCleaner2</c> do contrato com o RVM.UI, desenhado com o <c>vacuum-cleaner</c> do Tabler.</summary>
    SmartVacuumCleaner2,

    /// <summary>Icone <c>Smartphone</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile</c> do Tabler.</summary>
    Smartphone,

    /// <summary>Icone <c>Smartphone2</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile</c> do Tabler.</summary>
    Smartphone2,

    /// <summary>Icone <c>SmartphoneRotate2</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile-rotated</c> do Tabler.</summary>
    SmartphoneRotate2,

    /// <summary>Icone <c>SmartphoneRotateAngle</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile-rotated</c> do Tabler.</summary>
    SmartphoneRotateAngle,

    /// <summary>Icone <c>SmartphoneRotateOrientation</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile-rotated</c> do Tabler.</summary>
    SmartphoneRotateOrientation,

    /// <summary>Icone <c>SmartphoneUpdate</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile-down</c> do Tabler.</summary>
    SmartphoneUpdate,

    /// <summary>Icone <c>SmartphoneVibration</c> do contrato com o RVM.UI, desenhado com o <c>device-mobile-vibration</c> do Tabler.</summary>
    SmartphoneVibration,

    /// <summary>Icone <c>SmileCircle</c> do contrato com o RVM.UI, desenhado com o <c>mood-smile</c> do Tabler.</summary>
    SmileCircle,

    /// <summary>Icone <c>SmileSquare</c> do contrato com o RVM.UI, desenhado com o <c>mood-smile</c> do Tabler.</summary>
    SmileSquare,

    /// <summary>Icone <c>Snowflake</c> do contrato com o RVM.UI, desenhado com o <c>snowflake</c> do Tabler.</summary>
    Snowflake,

    /// <summary>Icone <c>Socket</c> do contrato com o RVM.UI, desenhado com o <c>plug</c> do Tabler.</summary>
    Socket,

    /// <summary>Icone <c>Sofa</c> do contrato com o RVM.UI, desenhado com o <c>sofa</c> do Tabler.</summary>
    Sofa,

    /// <summary>Icone <c>Sofa2</c> do contrato com o RVM.UI, desenhado com o <c>armchair</c> do Tabler.</summary>
    Sofa2,

    /// <summary>Icone <c>Sofa3</c> do contrato com o RVM.UI, desenhado com o <c>armchair-2</c> do Tabler.</summary>
    Sofa3,

    /// <summary>Icone <c>Sort</c> do contrato com o RVM.UI, desenhado com o <c>arrows-sort</c> do Tabler.</summary>
    Sort,

    /// <summary>Icone <c>SortByAlphabet</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending-letters</c> do Tabler.</summary>
    SortByAlphabet,

    /// <summary>Icone <c>SortByTime</c> do contrato com o RVM.UI, desenhado com o <c>clock</c> do Tabler.</summary>
    SortByTime,

    /// <summary>Icone <c>SortEssentionalUi</c> do contrato com o RVM.UI, desenhado com o <c>arrows-sort</c> do Tabler.</summary>
    SortEssentionalUi,

    /// <summary>Icone <c>SortFromBottomToTop</c> do contrato com o RVM.UI, desenhado com o <c>sort-ascending</c> do Tabler.</summary>
    SortFromBottomToTop,

    /// <summary>Icone <c>SortFromTopToBottom</c> do contrato com o RVM.UI, desenhado com o <c>sort-descending</c> do Tabler.</summary>
    SortFromTopToBottom,

    /// <summary>Icone <c>SortHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-left-right</c> do Tabler.</summary>
    SortHorizontal,

    /// <summary>Icone <c>SortVertical</c> do contrato com o RVM.UI, desenhado com o <c>arrows-up-down</c> do Tabler.</summary>
    SortVertical,

    /// <summary>Icone <c>Soundwave</c> do contrato com o RVM.UI, desenhado com o <c>wave-sine</c> do Tabler.</summary>
    Soundwave,

    /// <summary>Icone <c>SoundwaveVideoAudioSound</c> do contrato com o RVM.UI, desenhado com o <c>wave-sine</c> do Tabler.</summary>
    SoundwaveVideoAudioSound,

    /// <summary>Icone <c>SoundwaveVideoAudioSound2</c> do contrato com o RVM.UI, desenhado com o <c>wave-sine</c> do Tabler.</summary>
    SoundwaveVideoAudioSound2,

    /// <summary>Icone <c>Speaker</c> do contrato com o RVM.UI, desenhado com o <c>device-speaker</c> do Tabler.</summary>
    Speaker,

    /// <summary>Icone <c>SpeakerMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>volume</c> do Tabler.</summary>
    SpeakerMinimalistic,

    /// <summary>Icone <c>SpecialEffects</c> do contrato com o RVM.UI, desenhado com o <c>sparkles</c> do Tabler.</summary>
    SpecialEffects,

    /// <summary>Icone <c>SpedometerLow</c> do contrato com o RVM.UI, desenhado com o <c>gauge</c> do Tabler.</summary>
    SpedometerLow,

    /// <summary>Icone <c>SpedometerMax</c> do contrato com o RVM.UI, desenhado com o <c>gauge</c> do Tabler.</summary>
    SpedometerMax,

    /// <summary>Icone <c>SpedometerMiddle</c> do contrato com o RVM.UI, desenhado com o <c>gauge</c> do Tabler.</summary>
    SpedometerMiddle,

    /// <summary>Icone <c>SquareAcademicCap</c> do contrato com o RVM.UI, desenhado com o <c>school</c> do Tabler.</summary>
    SquareAcademicCap,

    /// <summary>Icone <c>SquareAcademicCap2</c> do contrato com o RVM.UI, desenhado com o <c>school</c> do Tabler.</summary>
    SquareAcademicCap2,

    /// <summary>Icone <c>SquareAltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>square-chevron-down</c> do Tabler.</summary>
    SquareAltArrowDown,

    /// <summary>Icone <c>SquareAltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>square-chevron-left</c> do Tabler.</summary>
    SquareAltArrowLeft,

    /// <summary>Icone <c>SquareAltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>square-chevron-right</c> do Tabler.</summary>
    SquareAltArrowRight,

    /// <summary>Icone <c>SquareAltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>square-chevron-up</c> do Tabler.</summary>
    SquareAltArrowUp,

    /// <summary>Icone <c>SquareArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    SquareArrowDown,

    /// <summary>Icone <c>SquareArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-left</c> do Tabler.</summary>
    SquareArrowLeft,

    /// <summary>Icone <c>SquareArrowLeftDown</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    SquareArrowLeftDown,

    /// <summary>Icone <c>SquareArrowLeftUp</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-left</c> do Tabler.</summary>
    SquareArrowLeftUp,

    /// <summary>Icone <c>SquareArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-right</c> do Tabler.</summary>
    SquareArrowRight,

    /// <summary>Icone <c>SquareArrowRightDown</c> do contrato com o RVM.UI, desenhado com o <c>arrow-down-right</c> do Tabler.</summary>
    SquareArrowRightDown,

    /// <summary>Icone <c>SquareArrowRightUp</c> do contrato com o RVM.UI, desenhado com o <c>arrow-up-right</c> do Tabler.</summary>
    SquareArrowRightUp,

    /// <summary>Icone <c>SquareArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-up</c> do Tabler.</summary>
    SquareArrowUp,

    /// <summary>Icone <c>SquareBottomDown</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    SquareBottomDown,

    /// <summary>Icone <c>SquareBottomUp</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-up</c> do Tabler.</summary>
    SquareBottomUp,

    /// <summary>Icone <c>SquareDoubleAltArrowDown</c> do contrato com o RVM.UI, desenhado com o <c>square-chevrons-down</c> do Tabler.</summary>
    SquareDoubleAltArrowDown,

    /// <summary>Icone <c>SquareDoubleAltArrowLeft</c> do contrato com o RVM.UI, desenhado com o <c>square-chevrons-left</c> do Tabler.</summary>
    SquareDoubleAltArrowLeft,

    /// <summary>Icone <c>SquareDoubleAltArrowRight</c> do contrato com o RVM.UI, desenhado com o <c>square-chevrons-right</c> do Tabler.</summary>
    SquareDoubleAltArrowRight,

    /// <summary>Icone <c>SquareDoubleAltArrowUp</c> do contrato com o RVM.UI, desenhado com o <c>square-chevrons-up</c> do Tabler.</summary>
    SquareDoubleAltArrowUp,

    /// <summary>Icone <c>SquareForward</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-right</c> do Tabler.</summary>
    SquareForward,

    /// <summary>Icone <c>SquareShareLine</c> do contrato com o RVM.UI, desenhado com o <c>share</c> do Tabler.</summary>
    SquareShareLine,

    /// <summary>Icone <c>SquareSortHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-left-right</c> do Tabler.</summary>
    SquareSortHorizontal,

    /// <summary>Icone <c>SquareSortVertical</c> do contrato com o RVM.UI, desenhado com o <c>arrows-up-down</c> do Tabler.</summary>
    SquareSortVertical,

    /// <summary>Icone <c>SquareTopDown</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-down</c> do Tabler.</summary>
    SquareTopDown,

    /// <summary>Icone <c>SquareTopUp</c> do contrato com o RVM.UI, desenhado com o <c>square-arrow-up</c> do Tabler.</summary>
    SquareTopUp,

    /// <summary>Icone <c>SquareTransferHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-exchange</c> do Tabler.</summary>
    SquareTransferHorizontal,

    /// <summary>Icone <c>SquareTransferVertical</c> do contrato com o RVM.UI, desenhado com o <c>arrows-exchange-2</c> do Tabler.</summary>
    SquareTransferVertical,

    /// <summary>Icone <c>SsdRound</c> do contrato com o RVM.UI, desenhado com o <c>device-floppy</c> do Tabler.</summary>
    SsdRound,

    /// <summary>Icone <c>SsdSquare</c> do contrato com o RVM.UI, desenhado com o <c>device-sd-card</c> do Tabler.</summary>
    SsdSquare,

    /// <summary>Icone <c>StarAngle</c> do contrato com o RVM.UI, desenhado com o <c>star</c> do Tabler.</summary>
    StarAngle,

    /// <summary>Icone <c>StarAstronomy</c> do contrato com o RVM.UI, desenhado com o <c>star</c> do Tabler.</summary>
    StarAstronomy,

    /// <summary>Icone <c>StarCircle</c> do contrato com o RVM.UI, desenhado com o <c>star</c> do Tabler.</summary>
    StarCircle,

    /// <summary>Icone <c>StarFall</c> do contrato com o RVM.UI, desenhado com o <c>meteor</c> do Tabler.</summary>
    StarFall,

    /// <summary>Icone <c>StarFall2</c> do contrato com o RVM.UI, desenhado com o <c>meteor</c> do Tabler.</summary>
    StarFall2,

    /// <summary>Icone <c>StarFallMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>meteor</c> do Tabler.</summary>
    StarFallMinimalistic,

    /// <summary>Icone <c>StarFallMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>meteor</c> do Tabler.</summary>
    StarFallMinimalistic2,

    /// <summary>Icone <c>StarRainbow</c> do contrato com o RVM.UI, desenhado com o <c>rainbow</c> do Tabler.</summary>
    StarRainbow,

    /// <summary>Icone <c>StarRing</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    StarRing,

    /// <summary>Icone <c>StarRings</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    StarRings,

    /// <summary>Icone <c>StarShine</c> do contrato com o RVM.UI, desenhado com o <c>sparkles</c> do Tabler.</summary>
    StarShine,

    /// <summary>Icone <c>Stars</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    Stars,

    /// <summary>Icone <c>StarsAstronomy</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    StarsAstronomy,

    /// <summary>Icone <c>StarsLine</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    StarsLine,

    /// <summary>Icone <c>StarsMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>stars</c> do Tabler.</summary>
    StarsMinimalistic,

    /// <summary>Icone <c>Station</c> do contrato com o RVM.UI, desenhado com o <c>antenna</c> do Tabler.</summary>
    Station,

    /// <summary>Icone <c>StationMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>antenna</c> do Tabler.</summary>
    StationMinimalistic,

    /// <summary>Icone <c>Stethoscope</c> do contrato com o RVM.UI, desenhado com o <c>stethoscope</c> do Tabler.</summary>
    Stethoscope,

    /// <summary>Icone <c>StickerCircle</c> do contrato com o RVM.UI, desenhado com o <c>sticker</c> do Tabler.</summary>
    StickerCircle,

    /// <summary>Icone <c>StickerSmileCircle</c> do contrato com o RVM.UI, desenhado com o <c>sticker-smile</c> do Tabler.</summary>
    StickerSmileCircle,

    /// <summary>Icone <c>StickerSmileCircle2</c> do contrato com o RVM.UI, desenhado com o <c>sticker-smile</c> do Tabler.</summary>
    StickerSmileCircle2,

    /// <summary>Icone <c>StickerSmileSquare</c> do contrato com o RVM.UI, desenhado com o <c>sticker-smile</c> do Tabler.</summary>
    StickerSmileSquare,

    /// <summary>Icone <c>StickerSquare</c> do contrato com o RVM.UI, desenhado com o <c>sticker-2</c> do Tabler.</summary>
    StickerSquare,

    /// <summary>Icone <c>Stop</c> do contrato com o RVM.UI, desenhado com o <c>player-stop</c> do Tabler.</summary>
    Stop,

    /// <summary>Icone <c>StopCircle</c> do contrato com o RVM.UI, desenhado com o <c>player-stop</c> do Tabler.</summary>
    StopCircle,

    /// <summary>Icone <c>Stopwatch</c> do contrato com o RVM.UI, desenhado com o <c>stopwatch</c> do Tabler.</summary>
    Stopwatch,

    /// <summary>Icone <c>StopwatchPause</c> do contrato com o RVM.UI, desenhado com o <c>stopwatch</c> do Tabler.</summary>
    StopwatchPause,

    /// <summary>Icone <c>StopwatchPlay</c> do contrato com o RVM.UI, desenhado com o <c>stopwatch</c> do Tabler.</summary>
    StopwatchPlay,

    /// <summary>Icone <c>Stream</c> do contrato com o RVM.UI, desenhado com o <c>wifi</c> do Tabler.</summary>
    Stream,

    /// <summary>Icone <c>Streets</c> do contrato com o RVM.UI, desenhado com o <c>road</c> do Tabler.</summary>
    Streets,

    /// <summary>Icone <c>StreetsMapPoint</c> do contrato com o RVM.UI, desenhado com o <c>map-pin</c> do Tabler.</summary>
    StreetsMapPoint,

    /// <summary>Icone <c>StreetsNavigation</c> do contrato com o RVM.UI, desenhado com o <c>route</c> do Tabler.</summary>
    StreetsNavigation,

    /// <summary>Icone <c>Stretching</c> do contrato com o RVM.UI, desenhado com o <c>stretching</c> do Tabler.</summary>
    Stretching,

    /// <summary>Icone <c>StretchingRound</c> do contrato com o RVM.UI, desenhado com o <c>stretching</c> do Tabler.</summary>
    StretchingRound,

    /// <summary>Icone <c>Structure</c> do contrato com o RVM.UI, desenhado com o <c>topology-star-3</c> do Tabler.</summary>
    Structure,

    /// <summary>Icone <c>Subtitles</c> do contrato com o RVM.UI, desenhado com o <c>subtitles</c> do Tabler.</summary>
    Subtitles,

    /// <summary>Icone <c>Suitcase</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    Suitcase,

    /// <summary>Icone <c>SuitcaseLines</c> do contrato com o RVM.UI, desenhado com o <c>briefcase-2</c> do Tabler.</summary>
    SuitcaseLines,

    /// <summary>Icone <c>SuitcaseTag</c> do contrato com o RVM.UI, desenhado com o <c>briefcase</c> do Tabler.</summary>
    SuitcaseTag,

    /// <summary>Icone <c>Sun</c> do contrato com o RVM.UI, desenhado com o <c>sun</c> do Tabler.</summary>
    Sun,

    /// <summary>Icone <c>Sun2</c> do contrato com o RVM.UI, desenhado com o <c>sun</c> do Tabler.</summary>
    Sun2,

    /// <summary>Icone <c>SunFog</c> do contrato com o RVM.UI, desenhado com o <c>sun-wind</c> do Tabler.</summary>
    SunFog,

    /// <summary>Icone <c>Sunrise</c> do contrato com o RVM.UI, desenhado com o <c>sunrise</c> do Tabler.</summary>
    Sunrise,

    /// <summary>Icone <c>Sunset</c> do contrato com o RVM.UI, desenhado com o <c>sunset</c> do Tabler.</summary>
    Sunset,

    /// <summary>Icone <c>Suspension</c> do contrato com o RVM.UI, desenhado com o <c>settings</c> do Tabler.</summary>
    Suspension,

    /// <summary>Icone <c>SuspensionBolt</c> do contrato com o RVM.UI, desenhado com o <c>settings-bolt</c> do Tabler.</summary>
    SuspensionBolt,

    /// <summary>Icone <c>SuspensionCross</c> do contrato com o RVM.UI, desenhado com o <c>settings-x</c> do Tabler.</summary>
    SuspensionCross,

    /// <summary>Icone <c>Swimming</c> do contrato com o RVM.UI, desenhado com o <c>swimming</c> do Tabler.</summary>
    Swimming,

    /// <summary>Icone <c>Syringe</c> do contrato com o RVM.UI, desenhado com o <c>needle</c> do Tabler.</summary>
    Syringe,

    /// <summary>Icone <c>TShirt</c> do contrato com o RVM.UI, desenhado com o <c>shirt</c> do Tabler.</summary>
    TShirt,

    /// <summary>Icone <c>Tablet</c> do contrato com o RVM.UI, desenhado com o <c>device-tablet</c> do Tabler.</summary>
    Tablet,

    /// <summary>Icone <c>Tag</c> do contrato com o RVM.UI, desenhado com o <c>tag</c> do Tabler.</summary>
    Tag,

    /// <summary>Icone <c>TagHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>tag</c> do Tabler.</summary>
    TagHorizontal,

    /// <summary>Icone <c>TagPrice</c> do contrato com o RVM.UI, desenhado com o <c>tag</c> do Tabler.</summary>
    TagPrice,

    /// <summary>Icone <c>Target</c> do contrato com o RVM.UI, desenhado com o <c>target</c> do Tabler.</summary>
    Target,

    /// <summary>Icone <c>TeaCup</c> do contrato com o RVM.UI, desenhado com o <c>cup</c> do Tabler.</summary>
    TeaCup,

    /// <summary>Icone <c>Telescope</c> do contrato com o RVM.UI, desenhado com o <c>telescope</c> do Tabler.</summary>
    Telescope,

    /// <summary>Icone <c>Temperature</c> do contrato com o RVM.UI, desenhado com o <c>temperature</c> do Tabler.</summary>
    Temperature,

    /// <summary>Icone <c>Tennis</c> do contrato com o RVM.UI, desenhado com o <c>ball-tennis</c> do Tabler.</summary>
    Tennis,

    /// <summary>Icone <c>Tennis2</c> do contrato com o RVM.UI, desenhado com o <c>ball-tennis</c> do Tabler.</summary>
    Tennis2,

    /// <summary>Icone <c>TestTube</c> do contrato com o RVM.UI, desenhado com o <c>test-pipe</c> do Tabler.</summary>
    TestTube,

    /// <summary>Icone <c>TestTubeMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>test-pipe-2</c> do Tabler.</summary>
    TestTubeMinimalistic,

    /// <summary>Icone <c>Text</c> do contrato com o RVM.UI, desenhado com o <c>typography</c> do Tabler.</summary>
    Text,

    /// <summary>Icone <c>TextBold</c> do contrato com o RVM.UI, desenhado com o <c>bold</c> do Tabler.</summary>
    TextBold,

    /// <summary>Icone <c>TextBoldCircle</c> do contrato com o RVM.UI, desenhado com o <c>bold</c> do Tabler.</summary>
    TextBoldCircle,

    /// <summary>Icone <c>TextBoldSquare</c> do contrato com o RVM.UI, desenhado com o <c>bold</c> do Tabler.</summary>
    TextBoldSquare,

    /// <summary>Icone <c>TextCircle</c> do contrato com o RVM.UI, desenhado com o <c>typography</c> do Tabler.</summary>
    TextCircle,

    /// <summary>Icone <c>TextCross</c> do contrato com o RVM.UI, desenhado com o <c>typography-off</c> do Tabler.</summary>
    TextCross,

    /// <summary>Icone <c>TextCrossCircle</c> do contrato com o RVM.UI, desenhado com o <c>typography-off</c> do Tabler.</summary>
    TextCrossCircle,

    /// <summary>Icone <c>TextCrossSquare</c> do contrato com o RVM.UI, desenhado com o <c>typography-off</c> do Tabler.</summary>
    TextCrossSquare,

    /// <summary>Icone <c>TextField</c> do contrato com o RVM.UI, desenhado com o <c>forms</c> do Tabler.</summary>
    TextField,

    /// <summary>Icone <c>TextFieldFocus</c> do contrato com o RVM.UI, desenhado com o <c>forms</c> do Tabler.</summary>
    TextFieldFocus,

    /// <summary>Icone <c>TextItalic</c> do contrato com o RVM.UI, desenhado com o <c>italic</c> do Tabler.</summary>
    TextItalic,

    /// <summary>Icone <c>TextItalicCircle</c> do contrato com o RVM.UI, desenhado com o <c>italic</c> do Tabler.</summary>
    TextItalicCircle,

    /// <summary>Icone <c>TextItalicSquare</c> do contrato com o RVM.UI, desenhado com o <c>italic</c> do Tabler.</summary>
    TextItalicSquare,

    /// <summary>Icone <c>TextSelection</c> do contrato com o RVM.UI, desenhado com o <c>text-recognition</c> do Tabler.</summary>
    TextSelection,

    /// <summary>Icone <c>TextSquare</c> do contrato com o RVM.UI, desenhado com o <c>typography</c> do Tabler.</summary>
    TextSquare,

    /// <summary>Icone <c>TextSquare2</c> do contrato com o RVM.UI, desenhado com o <c>typography</c> do Tabler.</summary>
    TextSquare2,

    /// <summary>Icone <c>TextUnderline</c> do contrato com o RVM.UI, desenhado com o <c>underline</c> do Tabler.</summary>
    TextUnderline,

    /// <summary>Icone <c>TextUnderlineCircle</c> do contrato com o RVM.UI, desenhado com o <c>underline</c> do Tabler.</summary>
    TextUnderlineCircle,

    /// <summary>Icone <c>TextUnderlineCross</c> do contrato com o RVM.UI, desenhado com o <c>underline-off</c> do Tabler.</summary>
    TextUnderlineCross,

    /// <summary>Icone <c>Thermometer</c> do contrato com o RVM.UI, desenhado com o <c>thermometer</c> do Tabler.</summary>
    Thermometer,

    /// <summary>Icone <c>ThreeSquares</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    ThreeSquares,

    /// <summary>Icone <c>TickerStar</c> do contrato com o RVM.UI, desenhado com o <c>star</c> do Tabler.</summary>
    TickerStar,

    /// <summary>Icone <c>Ticket</c> do contrato com o RVM.UI, desenhado com o <c>ticket</c> do Tabler.</summary>
    Ticket,

    /// <summary>Icone <c>TicketSale</c> do contrato com o RVM.UI, desenhado com o <c>ticket</c> do Tabler.</summary>
    TicketSale,

    /// <summary>Icone <c>ToPip</c> do contrato com o RVM.UI, desenhado com o <c>picture-in-picture</c> do Tabler.</summary>
    ToPip,

    /// <summary>Icone <c>Tornado</c> do contrato com o RVM.UI, desenhado com o <c>tornado</c> do Tabler.</summary>
    Tornado,

    /// <summary>Icone <c>TornadoSmall</c> do contrato com o RVM.UI, desenhado com o <c>tornado</c> do Tabler.</summary>
    TornadoSmall,

    /// <summary>Icone <c>Traffic</c> do contrato com o RVM.UI, desenhado com o <c>traffic-lights</c> do Tabler.</summary>
    Traffic,

    /// <summary>Icone <c>TrafficEconomy</c> do contrato com o RVM.UI, desenhado com o <c>traffic-lights</c> do Tabler.</summary>
    TrafficEconomy,

    /// <summary>Icone <c>Tram</c> do contrato com o RVM.UI, desenhado com o <c>train</c> do Tabler.</summary>
    Tram,

    /// <summary>Icone <c>TransferHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>arrows-exchange</c> do Tabler.</summary>
    TransferHorizontal,

    /// <summary>Icone <c>TransferVertical</c> do contrato com o RVM.UI, desenhado com o <c>transfer-vertical</c> do Tabler.</summary>
    TransferVertical,

    /// <summary>Icone <c>Translation</c> do contrato com o RVM.UI, desenhado com o <c>language</c> do Tabler.</summary>
    Translation,

    /// <summary>Icone <c>Translation2</c> do contrato com o RVM.UI, desenhado com o <c>language</c> do Tabler.</summary>
    Translation2,

    /// <summary>Icone <c>Transmission</c> do contrato com o RVM.UI, desenhado com o <c>settings</c> do Tabler.</summary>
    Transmission,

    /// <summary>Icone <c>TransmissionCircle</c> do contrato com o RVM.UI, desenhado com o <c>settings</c> do Tabler.</summary>
    TransmissionCircle,

    /// <summary>Icone <c>TransmissionSquare</c> do contrato com o RVM.UI, desenhado com o <c>settings</c> do Tabler.</summary>
    TransmissionSquare,

    /// <summary>Icone <c>TrashBin</c> do contrato com o RVM.UI, desenhado com o <c>trash</c> do Tabler.</summary>
    TrashBin,

    /// <summary>Icone <c>TrashBin2</c> do contrato com o RVM.UI, desenhado com o <c>trash</c> do Tabler.</summary>
    TrashBin2,

    /// <summary>Icone <c>TrashBinMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>trash</c> do Tabler.</summary>
    TrashBinMinimalistic,

    /// <summary>Icone <c>TrashBinMinimalistic2</c> do contrato com o RVM.UI, desenhado com o <c>trash</c> do Tabler.</summary>
    TrashBinMinimalistic2,

    /// <summary>Icone <c>Treadmill</c> do contrato com o RVM.UI, desenhado com o <c>treadmill</c> do Tabler.</summary>
    Treadmill,

    /// <summary>Icone <c>TreadmillRound</c> do contrato com o RVM.UI, desenhado com o <c>treadmill</c> do Tabler.</summary>
    TreadmillRound,

    /// <summary>Icone <c>Trellis</c> do contrato com o RVM.UI, desenhado com o <c>fence</c> do Tabler.</summary>
    Trellis,

    /// <summary>Icone <c>Tuning</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    Tuning,

    /// <summary>Icone <c>Tuning2</c> do contrato com o RVM.UI, desenhado com o <c>adjustments-horizontal</c> do Tabler.</summary>
    Tuning2,

    /// <summary>Icone <c>Tuning3</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    Tuning3,

    /// <summary>Icone <c>Tuning4</c> do contrato com o RVM.UI, desenhado com o <c>adjustments-horizontal</c> do Tabler.</summary>
    Tuning4,

    /// <summary>Icone <c>TuningSquare</c> do contrato com o RVM.UI, desenhado com o <c>adjustments</c> do Tabler.</summary>
    TuningSquare,

    /// <summary>Icone <c>TuningSquare2</c> do contrato com o RVM.UI, desenhado com o <c>adjustments-horizontal</c> do Tabler.</summary>
    TuningSquare2,

    /// <summary>Icone <c>Turntable</c> do contrato com o RVM.UI, desenhado com o <c>vinyl</c> do Tabler.</summary>
    Turntable,

    /// <summary>Icone <c>TurntableMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>vinyl</c> do Tabler.</summary>
    TurntableMinimalistic,

    /// <summary>Icone <c>TurntableMusicNote</c> do contrato com o RVM.UI, desenhado com o <c>vinyl</c> do Tabler.</summary>
    TurntableMusicNote,

    /// <summary>Icone <c>Tv</c> do contrato com o RVM.UI, desenhado com o <c>device-tv</c> do Tabler.</summary>
    Tv,

    /// <summary>Icone <c>Ufo</c> do contrato com o RVM.UI, desenhado com o <c>ufo</c> do Tabler.</summary>
    Ufo,

    /// <summary>Icone <c>Ufo2</c> do contrato com o RVM.UI, desenhado com o <c>ufo</c> do Tabler.</summary>
    Ufo2,

    /// <summary>Icone <c>Ufo3</c> do contrato com o RVM.UI, desenhado com o <c>ufo</c> do Tabler.</summary>
    Ufo3,

    /// <summary>Icone <c>Umbrella</c> do contrato com o RVM.UI, desenhado com o <c>umbrella</c> do Tabler.</summary>
    Umbrella,

    /// <summary>Icone <c>UndoLeft</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    UndoLeft,

    /// <summary>Icone <c>UndoLeftRound</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    UndoLeftRound,

    /// <summary>Icone <c>UndoLeftRoundSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    UndoLeftRoundSquare,

    /// <summary>Icone <c>UndoLeftSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrow-back-up</c> do Tabler.</summary>
    UndoLeftSquare,

    /// <summary>Icone <c>UndoRight</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    UndoRight,

    /// <summary>Icone <c>UndoRightRound</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    UndoRightRound,

    /// <summary>Icone <c>UndoRightRoundArrowsAction</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    UndoRightRoundArrowsAction,

    /// <summary>Icone <c>UndoRightSquare</c> do contrato com o RVM.UI, desenhado com o <c>arrow-forward-up</c> do Tabler.</summary>
    UndoRightSquare,

    /// <summary>Icone <c>Unread</c> do contrato com o RVM.UI, desenhado com o <c>mail</c> do Tabler.</summary>
    Unread,

    /// <summary>Icone <c>UploadMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>upload</c> do Tabler.</summary>
    UploadMinimalistic,

    /// <summary>Icone <c>UploadSquare</c> do contrato com o RVM.UI, desenhado com o <c>upload</c> do Tabler.</summary>
    UploadSquare,

    /// <summary>Icone <c>UploadTrack</c> do contrato com o RVM.UI, desenhado com o <c>upload</c> do Tabler.</summary>
    UploadTrack,

    /// <summary>Icone <c>UploadTrack2</c> do contrato com o RVM.UI, desenhado com o <c>upload</c> do Tabler.</summary>
    UploadTrack2,

    /// <summary>Icone <c>UploadTwiceSquare</c> do contrato com o RVM.UI, desenhado com o <c>upload</c> do Tabler.</summary>
    UploadTwiceSquare,

    /// <summary>Icone <c>Usb</c> do contrato com o RVM.UI, desenhado com o <c>usb</c> do Tabler.</summary>
    Usb,

    /// <summary>Icone <c>UsbCircle</c> do contrato com o RVM.UI, desenhado com o <c>usb</c> do Tabler.</summary>
    UsbCircle,

    /// <summary>Icone <c>UsbElectronicDevices</c> do contrato com o RVM.UI, desenhado com o <c>device-usb</c> do Tabler.</summary>
    UsbElectronicDevices,

    /// <summary>Icone <c>UsbSquare</c> do contrato com o RVM.UI, desenhado com o <c>usb</c> do Tabler.</summary>
    UsbSquare,

    /// <summary>Icone <c>UserBlock</c> do contrato com o RVM.UI, desenhado com o <c>user-cancel</c> do Tabler.</summary>
    UserBlock,

    /// <summary>Icone <c>UserBlockRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-cancel</c> do Tabler.</summary>
    UserBlockRounded,

    /// <summary>Icone <c>UserCheck</c> do contrato com o RVM.UI, desenhado com o <c>user-check</c> do Tabler.</summary>
    UserCheck,

    /// <summary>Icone <c>UserCheckRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-check</c> do Tabler.</summary>
    UserCheckRounded,

    /// <summary>Icone <c>UserCircle</c> do contrato com o RVM.UI, desenhado com o <c>user-circle</c> do Tabler.</summary>
    UserCircle,

    /// <summary>Icone <c>UserCross</c> do contrato com o RVM.UI, desenhado com o <c>user-x</c> do Tabler.</summary>
    UserCross,

    /// <summary>Icone <c>UserCrossRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-x</c> do Tabler.</summary>
    UserCrossRounded,

    /// <summary>Icone <c>UserHandUp</c> do contrato com o RVM.UI, desenhado com o <c>user-up</c> do Tabler.</summary>
    UserHandUp,

    /// <summary>Icone <c>UserHands</c> do contrato com o RVM.UI, desenhado com o <c>users</c> do Tabler.</summary>
    UserHands,

    /// <summary>Icone <c>UserHeart</c> do contrato com o RVM.UI, desenhado com o <c>user-heart</c> do Tabler.</summary>
    UserHeart,

    /// <summary>Icone <c>UserHeartRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-heart</c> do Tabler.</summary>
    UserHeartRounded,

    /// <summary>Icone <c>UserId</c> do contrato com o RVM.UI, desenhado com o <c>id</c> do Tabler.</summary>
    UserId,

    /// <summary>Icone <c>UserMinus</c> do contrato com o RVM.UI, desenhado com o <c>user-minus</c> do Tabler.</summary>
    UserMinus,

    /// <summary>Icone <c>UserMinusRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-minus</c> do Tabler.</summary>
    UserMinusRounded,

    /// <summary>Icone <c>UserPlus</c> do contrato com o RVM.UI, desenhado com o <c>user-plus</c> do Tabler.</summary>
    UserPlus,

    /// <summary>Icone <c>UserPlusRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-plus</c> do Tabler.</summary>
    UserPlusRounded,

    /// <summary>Icone <c>UserRounded</c> do contrato com o RVM.UI, desenhado com o <c>user</c> do Tabler.</summary>
    UserRounded,

    /// <summary>Icone <c>UserSpeak</c> do contrato com o RVM.UI, desenhado com o <c>user-screen</c> do Tabler.</summary>
    UserSpeak,

    /// <summary>Icone <c>UserSpeakRounded</c> do contrato com o RVM.UI, desenhado com o <c>user-screen</c> do Tabler.</summary>
    UserSpeakRounded,

    /// <summary>Icone <c>UsersGroupRounded</c> do contrato com o RVM.UI, desenhado com o <c>users-group</c> do Tabler.</summary>
    UsersGroupRounded,

    /// <summary>Icone <c>UsersGroupTwoRounded</c> do contrato com o RVM.UI, desenhado com o <c>users</c> do Tabler.</summary>
    UsersGroupTwoRounded,

    /// <summary>Icone <c>VerifiedCheck</c> do contrato com o RVM.UI, desenhado com o <c>rosette-discount-check</c> do Tabler.</summary>
    VerifiedCheck,

    /// <summary>Icone <c>VideoFrame</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    VideoFrame,

    /// <summary>Icone <c>VideoFrame2</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    VideoFrame2,

    /// <summary>Icone <c>VideoFrameCut</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    VideoFrameCut,

    /// <summary>Icone <c>VideoFrameCut2</c> do contrato com o RVM.UI, desenhado com o <c>movie</c> do Tabler.</summary>
    VideoFrameCut2,

    /// <summary>Icone <c>VideoFramePlayHorizontal</c> do contrato com o RVM.UI, desenhado com o <c>player-play</c> do Tabler.</summary>
    VideoFramePlayHorizontal,

    /// <summary>Icone <c>VideoFramePlayVertical</c> do contrato com o RVM.UI, desenhado com o <c>player-play</c> do Tabler.</summary>
    VideoFramePlayVertical,

    /// <summary>Icone <c>VideoFrameReplace</c> do contrato com o RVM.UI, desenhado com o <c>replace</c> do Tabler.</summary>
    VideoFrameReplace,

    /// <summary>Icone <c>VideoLibrary</c> do contrato com o RVM.UI, desenhado com o <c>playlist</c> do Tabler.</summary>
    VideoLibrary,

    /// <summary>Icone <c>Videocamera</c> do contrato com o RVM.UI, desenhado com o <c>video</c> do Tabler.</summary>
    Videocamera,

    /// <summary>Icone <c>VideocameraAdd</c> do contrato com o RVM.UI, desenhado com o <c>video-plus</c> do Tabler.</summary>
    VideocameraAdd,

    /// <summary>Icone <c>VideocameraRecord</c> do contrato com o RVM.UI, desenhado com o <c>player-record</c> do Tabler.</summary>
    VideocameraRecord,

    /// <summary>Icone <c>VinylRecord</c> do contrato com o RVM.UI, desenhado com o <c>vinyl</c> do Tabler.</summary>
    VinylRecord,

    /// <summary>Icone <c>Virus</c> do contrato com o RVM.UI, desenhado com o <c>virus</c> do Tabler.</summary>
    Virus,

    /// <summary>Icone <c>Volleyball</c> do contrato com o RVM.UI, desenhado com o <c>ball-volleyball</c> do Tabler.</summary>
    Volleyball,

    /// <summary>Icone <c>Volleyball2</c> do contrato com o RVM.UI, desenhado com o <c>ball-volleyball</c> do Tabler.</summary>
    Volleyball2,

    /// <summary>Icone <c>Volume</c> do contrato com o RVM.UI, desenhado com o <c>volume</c> do Tabler.</summary>
    Volume,

    /// <summary>Icone <c>VolumeCross</c> do contrato com o RVM.UI, desenhado com o <c>volume-off</c> do Tabler.</summary>
    VolumeCross,

    /// <summary>Icone <c>VolumeKnob</c> do contrato com o RVM.UI, desenhado com o <c>volume</c> do Tabler.</summary>
    VolumeKnob,

    /// <summary>Icone <c>VolumeLoud</c> do contrato com o RVM.UI, desenhado com o <c>volume</c> do Tabler.</summary>
    VolumeLoud,

    /// <summary>Icone <c>VolumeSmall</c> do contrato com o RVM.UI, desenhado com o <c>volume-3</c> do Tabler.</summary>
    VolumeSmall,

    /// <summary>Icone <c>WadOfMoney</c> do contrato com o RVM.UI, desenhado com o <c>cash</c> do Tabler.</summary>
    WadOfMoney,

    /// <summary>Icone <c>Walking</c> do contrato com o RVM.UI, desenhado com o <c>walk</c> do Tabler.</summary>
    Walking,

    /// <summary>Icone <c>WalkingRound</c> do contrato com o RVM.UI, desenhado com o <c>walk</c> do Tabler.</summary>
    WalkingRound,

    /// <summary>Icone <c>Wallet</c> do contrato com o RVM.UI, desenhado com o <c>wallet</c> do Tabler.</summary>
    Wallet,

    /// <summary>Icone <c>Wallet2</c> do contrato com o RVM.UI, desenhado com o <c>wallet</c> do Tabler.</summary>
    Wallet2,

    /// <summary>Icone <c>WalletMoney</c> do contrato com o RVM.UI, desenhado com o <c>wallet</c> do Tabler.</summary>
    WalletMoney,

    /// <summary>Icone <c>Wallpaper</c> do contrato com o RVM.UI, desenhado com o <c>wallpaper</c> do Tabler.</summary>
    Wallpaper,

    /// <summary>Icone <c>WashingMachine</c> do contrato com o RVM.UI, desenhado com o <c>wash-machine</c> do Tabler.</summary>
    WashingMachine,

    /// <summary>Icone <c>WashingMachineMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>wash-machine</c> do Tabler.</summary>
    WashingMachineMinimalistic,

    /// <summary>Icone <c>WatchRound</c> do contrato com o RVM.UI, desenhado com o <c>device-watch</c> do Tabler.</summary>
    WatchRound,

    /// <summary>Icone <c>WatchSquare</c> do contrato com o RVM.UI, desenhado com o <c>device-watch</c> do Tabler.</summary>
    WatchSquare,

    /// <summary>Icone <c>WatchSquareMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>device-watch</c> do Tabler.</summary>
    WatchSquareMinimalistic,

    /// <summary>Icone <c>WatchSquareMinimalisticCharge</c> do contrato com o RVM.UI, desenhado com o <c>device-watch</c> do Tabler.</summary>
    WatchSquareMinimalisticCharge,

    /// <summary>Icone <c>Water</c> do contrato com o RVM.UI, desenhado com o <c>droplet</c> do Tabler.</summary>
    Water,

    /// <summary>Icone <c>WaterSun</c> do contrato com o RVM.UI, desenhado com o <c>droplet-half</c> do Tabler.</summary>
    WaterSun,

    /// <summary>Icone <c>Waterdrop</c> do contrato com o RVM.UI, desenhado com o <c>droplet</c> do Tabler.</summary>
    Waterdrop,

    /// <summary>Icone <c>Waterdrops</c> do contrato com o RVM.UI, desenhado com o <c>droplets</c> do Tabler.</summary>
    Waterdrops,

    /// <summary>Icone <c>Weigher</c> do contrato com o RVM.UI, desenhado com o <c>scale</c> do Tabler.</summary>
    Weigher,

    /// <summary>Icone <c>Wheel</c> do contrato com o RVM.UI, desenhado com o <c>wheel</c> do Tabler.</summary>
    Wheel,

    /// <summary>Icone <c>WheelAngle</c> do contrato com o RVM.UI, desenhado com o <c>steering-wheel</c> do Tabler.</summary>
    WheelAngle,

    /// <summary>Icone <c>Whisk</c> do contrato com o RVM.UI, desenhado com o <c>whisk</c> do Tabler.</summary>
    Whisk,

    /// <summary>Icone <c>WiFiRouter</c> do contrato com o RVM.UI, desenhado com o <c>router</c> do Tabler.</summary>
    WiFiRouter,

    /// <summary>Icone <c>WiFiRouterMinimalistic</c> do contrato com o RVM.UI, desenhado com o <c>router</c> do Tabler.</summary>
    WiFiRouterMinimalistic,

    /// <summary>Icone <c>WiFiRouterRound</c> do contrato com o RVM.UI, desenhado com o <c>router</c> do Tabler.</summary>
    WiFiRouterRound,

    /// <summary>Icone <c>Widget</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget,

    /// <summary>Icone <c>Widget2</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget2,

    /// <summary>Icone <c>Widget3</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget3,

    /// <summary>Icone <c>Widget4</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget4,

    /// <summary>Icone <c>Widget5</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget5,

    /// <summary>Icone <c>Widget6</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid</c> do Tabler.</summary>
    Widget6,

    /// <summary>Icone <c>WidgetAdd</c> do contrato com o RVM.UI, desenhado com o <c>layout-grid-add</c> do Tabler.</summary>
    WidgetAdd,

    /// <summary>Icone <c>Wind</c> do contrato com o RVM.UI, desenhado com o <c>wind</c> do Tabler.</summary>
    Wind,

    /// <summary>Icone <c>WindowFrame</c> do contrato com o RVM.UI, desenhado com o <c>window</c> do Tabler.</summary>
    WindowFrame,

    /// <summary>Icone <c>Wineglass</c> do contrato com o RVM.UI, desenhado com o <c>glass</c> do Tabler.</summary>
    Wineglass,

    /// <summary>Icone <c>WineglassTriangle</c> do contrato com o RVM.UI, desenhado com o <c>glass-cocktail</c> do Tabler.</summary>
    WineglassTriangle,

    /// <summary>Icone <c>Winrar</c> do contrato com o RVM.UI, desenhado com o <c>file-zip</c> do Tabler.</summary>
    Winrar,

    /// <summary>Icone <c>WirelessCharge</c> do contrato com o RVM.UI, desenhado com o <c>battery-charging</c> do Tabler.</summary>
    WirelessCharge,

    /// <summary>Icone <c>Women</c> do contrato com o RVM.UI, desenhado com o <c>woman</c> do Tabler.</summary>
    Women,

    /// <summary>Icone <c>Xxx</c> do contrato com o RVM.UI, desenhado com o <c>xxx</c> do Tabler.</summary>
    Xxx,

    /// <summary>Icone <c>ZipFile</c> do contrato com o RVM.UI, desenhado com o <c>file-zip</c> do Tabler.</summary>
    ZipFile
}
