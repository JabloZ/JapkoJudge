'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Challenge } from "@/lib/ClassTypes";
type ChallengesResult =
  | { success: true; challenges: Challenge[] }
  | { success: false; error: string };
  
export async function GetChallengesRequest({username}:{username:string}): Promise<ChallengesResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    
    const response=await fetch(`${process.env.BACKEND_URL}/api/users/${username}/challenges`,{
        method:"GET",
        headers:{
            "Authorization":`Bearer ${token}`
        }
    })
    
    if (!response.ok){
        const resp=await response.text();
        return {success:false,error:resp};
    }
    const data=await response.json();
    const challenges = data.challenges ?? [];
    
    return{success:true,challenges: challenges as Challenge[]};

}

export async function DeleteChallengeHandle(id:string, path:string){
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {error:"no token"};
    }
    
    try{
       
        const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/deleteChallenge`,{
            method:"POST",
            headers:{
                "Authorization":`Bearer ${token}`
            }
        });
        if (!response.ok){
            const err=await response.text();
            
            return {error:`Error! ${err}`}
        }
        
        const data=await response.json();
        
        
        //return {success: true, data};
    }
    catch(err){
        return {error: "Error: "+ (err as Error).message}
    }
    redirect(path);
}