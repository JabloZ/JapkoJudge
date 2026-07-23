
'use server';
import { getToken } from "@/lib/session";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";

export async function EditLanguageHandle(id:string, language:string, prevState:any,formData:FormData){
    //check if viewer is author
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {error:"no token"};
    }
    
    const startfile=formData.get("startfile") as File || null;
    const testfile=formData.get("testfile") as File || null;
    
    if (!startfile || startfile.size===0){
        return {error:"no startfile"};
    }
    if (!testfile || testfile.size===0){
        return {error:"no testfile"};
    }
    const ChallengeForm=new FormData();
    ChallengeForm.append("startfile",startfile);
    ChallengeForm.append("testfile", testfile);
    ChallengeForm.append("language", language);
    //todo 
    try{

        const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/editLanguage/${language}`,{
            method:"POST",
            headers:{
                "Authorization":`Bearer ${token}`,
                //no content type because file is not for json
            },
            body: ChallengeForm   
        });
        if (!response.ok){
            const err=await response.text();
            return {error:`Error! ${response.status}`}
        }
        const data=await response.json();
        
        return {success: true, data};
    }
    catch(err){
        return {error: "Error: "+ (err as Error).message}
    }
}