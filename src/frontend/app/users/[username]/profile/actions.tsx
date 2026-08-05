'use server';

import { getSession, getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { Profile } from "@/lib/ClassTypes";
type ProfileResult =
  | { success: true; profile: Profile }
  | { success: false; error: string };
  
export async function GetProfile({username}:{username:string}): Promise<ProfileResult> {
    
    const session = await getSession();
    if (!session) {
        redirect("/");
    }
    const token=await getToken();
    if(!token){
        return {success:false,error:"no token"};
    }

    
    const response=await fetch(`${process.env.BACKEND_URL}/api/users/${username}/profile`,{
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
    const profile = data.profile ?? null;
    
    return{success:true,profile:profile};

}

