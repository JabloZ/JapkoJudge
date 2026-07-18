'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Challenge } from "@/lib/ChallengeCard";
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
    
    const challenges = data.Challenges ?? data.challenges ?? [];
    
    
    return{success:true,challenges: challenges as Challenge[]};

}