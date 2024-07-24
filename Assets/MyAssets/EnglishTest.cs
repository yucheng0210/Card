using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnglishTest : MonoBehaviour
{
    private List<string> englishList = new List<string> { "passenger","ascend","climb","ladder","face","opposite","pedestrian","pavement","sweep","messy",
    "reach","cupboard","walk","leash","operate","lawn","observe","compete","insert","drawer","kneel","fireplace","survey","blossom","awning","curb",
    "sidewalk","countless","scatter","hang","railing","instrument","lean","diagonally","thin","trunk","numerous","locate","lead","fountain","stretch",
    "path","various","flier","bin","contain","immediately","surround","statue","barn","podium","hide","facade","plaque","mural","attire","tripod","infant",
    "crouch","knead","medicine","beverage","sore","throat","upset","emotion","humid","unbearable","ruin","fun","soothing","boring","matter","exaggerate",
    "allowance","chore","charge","include","direct","literature","stain","soak","mileage","efficient","vehicle","possess","alteration","cuff","fuel",
    "rent","carry","checkout","chop","view","restroom","stairs","veterinarian","vaccination","bar","probably","unusual","symmetrical","pain","chronic",
    "occupy","save","parallel","intersect","prescription","rarely","exhibition","controversial","distinguish","subtle","huge","brightly","confident",
    "particularly","revise","generous","aisle","available","rest","solve","treat","respect","bulky","fragile","doubt","moody","invade","disturb",
    "knowledge","clue","present","errand","furniture","donate","brochure","stack","pension","retirement","replace","similar","far-fetched","desperate",
    "agonize","awful","hire","executive","renew","contract","turnout","disappointing","effort","promation","negotiate","substantial","intend","fire",
    "purpose","lawsuit","competitor","resemble","procedure","permit","figure","estimate","hinder","progress","updated","detain","attorney","reliable",
    "accept","transfer","install","equipment","screen","potential","book","decline","sue","lawyer","justify","inform","balk","ambiguous","appear","ban",
    "beat","cheer","envy","fear","mend","obey","deny","utilize","melt","blame","crop","ache","estate","fluid","gauge","habit","headquarters","leak","length",
    "load","mercy","mischief","prank","cough","appearance","status","usage","vacant","temporary","entertaining","persuade","approve","expect","correct","accurate"
    ,"extra","detergent","amount","especially","belongings","intact","disarray","hate","electrician","ceiling","fix","company","ointment","smell","oad",
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
    "tune","competition","host","fomer","department","apply","saving","reduction" };
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
