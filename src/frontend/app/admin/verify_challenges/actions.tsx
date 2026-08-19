'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Challenge } from "@/lib/ClassTypes";
import { isAdmin } from "@/lib/session";
type ChallengesResult =
  | { success: true; challenges: Challenge[] }
  | { success: false; error: string };
  
export async function GetChallengesRequest(): Promise<ChallengesResult> {
   
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }
    const isUserAdmin= await isAdmin();
    if (!isUserAdmin){
        return {success:false,error:"you are not an admin"};
    }
    
    
    const response=await fetch(`${process.env.BACKEND_URL}/api/get/unverified_challenges`,{
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
type PostResult =
  | { success: true; message:string }
  | { success: false; error: string };

export async function PostChallengeDecision(id:string, decision:boolean): Promise<PostResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }
    const isUserAdmin= await isAdmin();
    if (!isUserAdmin){
        return {success:false,error:"you are not an admin"};
    }
    
    const response=await fetch(`${process.env.BACKEND_URL}/api/post/verify_challenge/${id}`,{
        method:"Post",
        headers:{
            "Authorization":`Bearer ${token}`,
            "Content-Type": "application/json"
        },
        body:JSON.stringify({decision})
    })
    
    if (!response.ok){
        
        const resp=await response.text();
        return {success:false,error:resp};
    }
    const data=await response.json();
    
    return{success:true,message:"ok"};

}
export async function PostChallengeDifficulty(id:string, difficulty:string): Promise<PostResult> {
    const num=Number(difficulty);
    if (!Number.isInteger(num) || num<1 || num>7){
        throw new Error("invalid");
    }
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }
     const isUserAdmin= await isAdmin();
    if (!isUserAdmin){
        return {success:false,error:"you are not an admin"};
    }
    
    
    
    const response=await fetch(`${process.env.BACKEND_URL}/api/post/change_difficulty/${id}`,{
        method:"Post",
        headers:{
            "Authorization":`Bearer ${token}`,
            "Content-Type": "application/json"
        },
        body:JSON.stringify({difficulty})
    })
    
    if (!response.ok){
        
        const resp=await response.text();
        return {success:false,error:resp};
    }
    const data=await response.json();
    
    return{success:true,message:"ok"};

}