using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnglishTest : MonoBehaviour
{
    private List<string> englishList = new List<string>
    { "passenger","ascend","climb","ladder","face","opposite","pedestrian","pavement","sweep","messy",
    "reach","cupboard","walk","leash","operate","lawn","observe","compete","insert","drawer","kneel","fireplace","survey","blossom","awning","curb",
    "sidewalk","countless","scatter","hang","railing","instrument","lean","diagonally","thin","trunk","numerous","locate","lead","fountain","stretch",
    "path","various","flier","bin","contain","immediately","surround","statue","barn","podium","hide","facade","plaque","mural","attire","tripod","infant",
    "crouch","knead","medicine","beverage","sore","throat","upset","emotion","humid","unbearable","ruin","fun","soothing","boring","matter","exaggerate",
    "allowance","chore","charge","include","direct","literature","stain","soak","mileage","efficient","vehicle","possess","alteration","cuff","fuel",
    "rent","carry","checkout","chop","view","restroom","stairs","veterinarian","vaccination","bar","probably","unusual","symmetrical","pain","chronic",
    "occupy","save","parallel","intersect","prescription","rarely","exhibition","controversial","distinguish","subtle","huge","brightly","confident",
    "particularly","revise","generous","aisle","available","rest","solve","treat","respect","bulky","fragile","doubt","moody","invade","disturb",
    "knowledge","clue","present","errand","furniture","donate","brochure","stack","pension","retirement","replace","similar","far-fetched","desperate",
    "agonize","awful","hire","executive","renew","contract","turnout","disappointing","effort","promotion","negotiate","substantial","intend","fire",
    "purpose","lawsuit","competitor","resemble","procedure","permit","figure","estimate","hinder","progress","updated","detain","attorney","reliable",
    "accept","transfer","install","equipment","screen","potential","book","decline","sue","lawyer","justify","inform","balk","ambiguous","appear","ban",
    "beat","cheer","envy","fear","mend","obey","deny","utilize","melt","blame","crop","ache","estate","fluid","gauge","habit","headquarters","leak","length",
    "load","mercy","mischief","prank","cough","appearance","status","usage","vacant","temporary","entertaining","persuade","approve","expect","correct","accurate"
    ,"extra","detergent","amount","especially","belongings","intact","disarray","hate","electrician","ceiling","fix","company","ointment","smell","odd",
    "price","neighbor","allow","assume","prohibit","slip","exchange","purchase","refund","realize","fare","prior","departure","result","diet","intake","consume",
    "impress","performance","improve","gradually","insurance","coverage","spouse","slightly","cause","device","malfunction","jam","calculator","prepare","expense",
    "actually","drench","strenuous","workout","bet","headline","concerning","obstruction","justice","determine","previous","sort","reward","reimbursement","bill",
    "submit","form","government","cost","billion","bid","suppose","accompany","commentary","editorial","immitate","budget","specify","increase","swamp","possibly",
    "overtime","sync","quit","unexpected","graduate","degree","celebration","attract","crowd","nephew","roughly","plumbing","setback","require","relocate",
    "operation","nearby","considerably","author","admire","work","pity","omit","belong","conclusion","vague","raise","wonder","demanding","rewarding",
    "reluctant","prefer","incredibly","concerned","deserve","credit","content","complain","willing","protest","brave","belief","appoint","committee",
    "announce","patience","mainstream","flagrantly","ignore","inaction","steep","cozy","cruel","flat","ideal","keen","dizzy","stout","splitting","somewhat",
    "publicly","frequently","directory","peculiar","inhale","symptom","abstract","cavity","counterfeit","afterwards","conduct","associate","evidently",
    "eventually","prospective","imply","modify","undertake","undergo","deficit","relevant","afford","behavior","flatter","mumble","legal","applicant",
    "pay","benefit","suffer","relieve","destination","handle","whole","annoying","ingredient","effectiveness","anticipate","aim","financial","invest",
    "tune","competition","host","former","department","apply","saving","reduction","governor","file","involve","administration","manufacture","adverse","publicity",
    "divulge","measure", "entire","authority","predict","investigate","robbery","worth","arrest","experience","significant","extremely","recover","warning",
    "effect","hazardous","bulletin","southbound","clog","head","moderate","commuter","stick","detour","artery","board", "cabin", "complimentary", "pleasant",
    "encounter", "turbulence", "momentarily", "fasten", "immigration", "claim", "customs", "declaration", "transit", "approximately", "rise", "suburban",
    "rapid", "halt", "further", "detail", "serve", "assigned", "itinerary", "assemble", "valuables", "precaution", "liable", "property", "unattended",
    "ticket", "tow", "domestic", "page", "contact", "identify", "close", "refrain", "litter", "scenic", "facility", "steadily", "reinforce", "nationwide",
    "recognition", "evacuate", "injured", "emergency", "auditorium", "award", "honor", "round", "applause", "following", "branch", "representative",
    "shortly", "acknowledge", "organize", "attend", "gratitude", "merchandise", "reputation", "tarnish", "avoid","elect","obscene","constitutional",
    "guarantee","review","strategy","minimal","objective","achieve","affect","remind","reserve","commitment","dedication","inspiration","surpass","gulf",
     "altitude","punctual","grocery","renovation","ward","condense","dismiss","infinite","dwindle","diagnose","speculate","abide", "adhere", "endorse",
    "cease", "denounce", "exert", "expire", "exploit", "extract", "scribble", "infer", "influence", "interfere", "prevail", "proceed", "sacrifice", "resist",
    "spread", "swell", "yield", "solicit", "accommodate", "confess", "pretend","eliminate", "collide", "divorce", "postpone", "frighten", "calculate", "commence",
    "penetrate", "deduct", "intervene", "emphasize", "abuse", "argument", "capital", "climate", "coalition", "compassion", "remark", "currency", "interruption",
    "insight", "drawback", "drought", "excursion","foundation", "institution", "defendant", "disorder", "intermission", "instinct", "outbreak", "principal",
    "pharmacy", "politician", "hemisphere", "surgeon", "remedy", "utensil", "treaty", "prejudice", "sequence", "heritage", "faculty", "offspring", "abrupt",
    "administrative", "hectic", "ashamed", "bilateral", "commemorative","concise", "considerate", "tragic", "dense", "eligible", "equivalent", "extinct",
    "industrious", "furious", "bankrupt", "mutual", "outstanding", "essential", "anxious", "flimsy", "fiscal", "subsidiary", "confidential", "precisely",
    "unanimously", "furthermore", "completely", "thus", "practically","definitely", "nonetheless", "enormously", "drastically", "continually", "adequately",
    "lubricant", "satisfy", "lodge", "violate", "specialize", "expand", "underutilized", "extensive", "consist", "depend", "transmission", "acquaint",
    "refreshment", "vary", "identical", "production", "disruption", "avert", "transition", "succeed", "construction", "concentrate", "appreciate", "rely",
    "initiate", "comprehensive","certify", "certificate", "certification", "certified", "criticize", "critic", "criticism", "critical", "destroy",
    "destroyer", "destruction", "destructive", "detect", "detection", "detective", "detector", "dominate", "dominant", "dominance", "domination",
    "intense", "intensive", "intensify", "intensity", "legislate", "legislation", "legislative", "legislature", "sensible", "sensitive", "sensory",
    "sensation", "stimulate", "stimulus", "stimulation", "stimulative","boost", "morale", "productivity", "current", "exceed", "disappear", "profit",
    "thought", "enclose", "utility", "commission", "authority", "restructure", "residential", "competitive", "necessitate", "ballot", "proxy", "quantity",
    "generic", "issue", "verify", "nullify", "prevent","naturally", "growth", "additional", "securities", "run", "consecutive", "accounting", "boardroom",
    "murder", "throughout", "disguise", "ordinary", "gather", "identity", "fellow", "reveal", "guess", "prize", "plant", "turf", "rake", "sod", "fertilizer",
    "earth", "lay", "firmly", "flush", "moisten", "mow","lid", "chest", "separately", "hinge", "glue", "drive", "nail", "varnish", "place", "latch", "remove",
    "loosen", "tilt", "drain", "sufficient", "rag", "disposal", "proper", "pour", "skillet", "grab", "burn", "cake", "surface", "scrub", "vigorously", "scour",
    "rinse", "rust"};
    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            int randomIndex = Random.Range(0, englishList.Count);
            Debug.Log(englishList[randomIndex]);
            englishList.RemoveAt(randomIndex);
        }
    }
}
