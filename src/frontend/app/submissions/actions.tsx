'use server';
import { getToken } from "@/lib/session";
import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { Manifest } from "@/lib/ClassTypes";
import { revalidatePath } from "next/cache";
import { Submission } from "@/lib/ClassTypes";
type GetDataResult= {success:false, error: string} | {success:true, submissions:Submission[]};
export async function GetSubmissions(id:string):Promise<GetDataResult>{
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {success:false, error:"no token"};
    }
    
    const response= await fetch(`${process.env.BACKEND_URL}/api/submissions/get`,{
        "method":"GET",
        "headers":{
            "Authorization":`Bearer ${token}`
        }
    });
    
    if (!response.ok){
        
        const err=await response.text();
        return{success:false,error:err};
    }
    const data=await response.json();
    const submissions = data.submissions ?? [];
    return{success:true,submissions:submissions as Submission[]};
}

