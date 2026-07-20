'use server';
import { getToken } from "@/lib/session";
import { redirect } from "next/navigation";

import { getSession } from "@/lib/session";
export async function EditChallengeGeneralRequest(id:string,prevState:any,formData:FormData){
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
    console.log(`${process.env.BACKEND_URL}/api/challenges/${id}/editGeneral`);
        const response=await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}/editGeneral`,{
            method:"POST",
            headers:{
                "Authorization":`Bearer ${token}`,
                
            },
            body: ChallengeForm   
        });
        console.log(response.status);
        if (!response.ok){//err heredh
            
            const err=await response.text();
            
            return {success:false,error:`Error! ${err}`}
        }
        
        const data=await response.json();
    

    return{success:true};
    //redirect(`/update_challenge/add_language/${challengeId}`);
}







type GetDataResult= {success:false, error: string} | {success:true, data:{title:string,description:string}};
export async function GetChallengeGeneralData(id:string):Promise<GetDataResult>{
    const session = await getSession();
        if (!session) {
            redirect("/");
        }
    const token=await getToken();
    if (!token){
        return {success:false, error:"no token"};
    }
    const response= await fetch(`${process.env.BACKEND_URL}/api/challenges/${id}`,{
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
    
    return{success:true,data:{title:data.title, description:data.description}};
}