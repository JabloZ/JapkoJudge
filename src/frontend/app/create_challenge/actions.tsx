'use server';
import { getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { AddLanguageForm } from "../update_challenge/add_language/[id]/AddLanguageForm";
import { getSession } from "@/lib/session";
export async function SendChallengeRequest(prevState:any,formData:FormData){
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {error:"no token"};
    }
    const title=formData.get("title")
    const description=formData.get("description")
    const ChallengeForm=new FormData();
    ChallengeForm.append("title",title as string);
    ChallengeForm.append("description", description as string);
    let challengeId;
    //todo 
    try{
        
        const response=await fetch(`${process.env.BACKEND_URL}/api/createChallenge`,{
            method:"POST",
            headers:{
                "Authorization":`Bearer ${token}`
            
            },
            body: ChallengeForm   
        });
        
        if (!response.ok){//err here
            
            const err=await response.text();
            return {error:`Error! ${err}`}
        }
        
        const data=await response.json();
        challengeId=data.challengeId;
        
    }
    catch(err){
        return {error: "Error: "+ (err as Error).message}
    }
    redirect(`/update_challenge/add_language/${challengeId}`);
}