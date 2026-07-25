'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Challenge } from "@/lib/ClassTypes";
type ChallengesResult =
  | { success: true; challenge: Challenge}
  | { success: false; error: string };
  
export async function GetChallengeRequest({id}:{id:string}): Promise<ChallengesResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}`,{
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
    
    
    const challenge = data ?? [];
    
    return{success:true,challenge: challenge as Challenge};

}